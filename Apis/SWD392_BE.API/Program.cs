using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Net.payOS;
using SWD392_BE.Repositories;
using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.Helper;
using SWD392_BE.Repositories.Interfaces;
using SWD392_BE.Repositories.Repositories;
using SWD392_BE.Repositories.Utils.ConfigOptions;
using SWD392_BE.Services;
using SWD392_BE.Services.Interfaces;
using SWD392_BE.Services.MapperProfile;
using SWD392_BE.Services.Services;
using SWD392_BE.Services.Services.Payment;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.Configure<GCSConfigOptions>(Configuration.GetSection("GCSConfigOptions"));
builder.Services.AddSingleton<PayOS>(provider =>
{
    string clientId = Configuration["PaymentEnvironment:PAYOS_CLIENT_ID"] ?? throw new Exception("Cannot find PAYOS_CLIENT_ID");
    string apiKey = Configuration["PaymentEnvironment:PAYOS_API_KEY"] ?? throw new Exception("Cannot find PAYOS_API_KEY");
    string checksumKey = Configuration["PaymentEnvironment:PAYOS_CHECKSUM_KEY"] ?? throw new Exception("Cannot find PAYOS_CHECKSUM_KEY");

    return new PayOS(clientId, apiKey, checksumKey);
});
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthorization();

builder.Services.AddAuthorization(options =>
{
    // Chính sách cho vai trò admin
    options.AddPolicy("Admin", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var user = context.User;
            var roleClaim = user.FindFirst("Role");
            if (roleClaim != null && roleClaim.Value == "1")
            {
                return true;
            }
            return false;
        });
    });

    // Chính sách cho vai trò customer
    options.AddPolicy("Customer", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var user = context.User;
            var roleClaim = user.FindFirst("Role");
            if (roleClaim != null && roleClaim.Value == "2")
            {
                return true;
            }
            return false;
        });
    });

    // Chính sách cho vai trò shipper
    options.AddPolicy("Shipper", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var user = context.User;
            var roleClaim = user.FindFirst("Role");
            if (roleClaim != null && roleClaim.Value == "3")
            {
                return true;
            }
            return false;
        });
    });

    // Chính sách cho các tài nguyên mà cả admin và customer có thể truy cập
    options.AddPolicy("AdminOrCustomerAccessPolicy", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var user = context.User;
            var roleClaim = user.FindFirst("Role");
            if (roleClaim != null && (roleClaim.Value == "1" || roleClaim.Value == "2"))
            {
                return true;
            }
            return false;
        });
    });

    // Chính sách cho các tài nguyên mà cả admin, customer, và shipper đều có thể truy cập
    options.AddPolicy("AdminCustomerOrShipperAccessPolicy", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var user = context.User;
            var roleClaim = user.FindFirst("Role");
            if (roleClaim != null && (roleClaim.Value == "1" || roleClaim.Value == "2" || roleClaim.Value == "3"))
            {
                return true;
            }
            return false;
        });
    });
});


// builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter the JWT token obtained from the login endpoint",
        Name = "Authorization"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
    // Lấy tệp XML cho chú thích Swagger
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});
builder.Services.AddAuthentication(item =>
{
    item.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    item.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(item =>
{
    item.RequireHttpsMetadata = true;
    item.SaveToken = true;
    item.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("c2VydmVwZXJmZWN0bHljaGVlc2VxdWlja2NvYWNoY29sbGVjdHNsb3Bld2lzZWNhbWU=")),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

//add automapper
builder.Services.AddAutoMapper(typeof(UserMapper));

builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.IgnoreNullValues = true;
        });
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IStoreRepository, StoreRepository>();
builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddHostedService<StoreStatusBackgroundService>();

builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<IStoreSessionRepository, StoreSessionRepository>();

builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IAreaService, AreaService>();

builder.Services.AddScoped<IFoodRepository, FoodRepository>();
builder.Services.AddScoped<IFoodService, FoodService>();

builder.Services.AddScoped<ICampusRepository, CampusRepository>();
builder.Services.AddScoped<ICampusService, CampusService>();

builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderServices>();

builder.Services.AddScoped<IOrderDetailsRepository, OrderDetailsRepository>();
builder.Services.AddScoped<IOrderDetailsServices, OrderDetailsServices>();

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>(); 
builder.Services.AddScoped<ITransactionService, TransactionService>();

builder.Services.AddScoped<IVoucherRepository, VoucherRepository>(); 
builder.Services.AddScoped<IVoucherService, VoucherService>();


builder.Services.AddScoped<IFirebaseStorageService, FirebaseStorageService>();

//Payment
builder.Services.AddScoped<IVnPayService, VnPayService>();
builder.Services.AddScoped<IPayOsServices, PayOsServices>();

builder.Services.AddSingleton(opt => StorageClient.Create(GoogleCredential.FromFile("..\\..\\nomnomfood-3f50b-firebase-adminsdk-pc2ef-7165f34c64.json")));
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpContextAccessor>().HttpContext!);
builder.Services.AddHttpContextAccessor();

// db local

builder.Services.AddDbContext<CampusFoodSystemContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("LocalDB"));
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("app-cors",
        builder =>
        {
            builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .WithExposedHeaders("X-Pagination")
            .AllowAnyMethod();
        });
});

//builder.Services.AddWebAPIService();




var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;

    var refreshHandler = serviceProvider.GetRequiredService<IRefreshTokenService>();

    refreshHandler.RemoveAllRefreshToken();
}

app.UseCors("app-cors");
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
