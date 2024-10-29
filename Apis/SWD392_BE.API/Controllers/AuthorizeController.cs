using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.ViewModels.ResultModel;
using SWD392_BE.Repositories.ViewModels.TokenViewModel;
using SWD392_BE.Services.Interfaces;
using SWD392_BE.Services.Sercurity;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SWD392_BE.API.Controllers
{
    [Route("api/v1/authorize")]
    [ApiController]
    [EnableCors("app-cors")]
    public class AuthorizeController : ControllerBase
    {
        private readonly IUserService _userServices;
        private readonly IRefreshTokenService _refreshHandler;
        private readonly IConfiguration _configuration;

        //private readonly IRefreshHandler refresh;

        public AuthorizeController(IUserService userServices, IRefreshTokenService refreshHandler, IConfiguration configuration)
        {
            _userServices = userServices;
            _refreshHandler = refreshHandler;
            _configuration = configuration;
        }
        #region GenerateToken
        /// <summary>
        /// Which will generating token accessible for user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [NonAction]
        public TokenViewModel GenerateToken(User user, String? RT)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim("UserId", user.UserId.ToString()),
                new Claim("UserName", user.Name),
                new Claim("Email", user.Email),
                new Claim("Role", user.Role.ToString())
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("c2VydmVwZXJmZWN0bHljaGVlc2VxdWlja2NvYWNoY29sbGVjdHNsb3Bld2lzZWNhbWU="));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "YourIssuer",
                audience: "YourAudience",
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            if (RT != null)
            {
                return new TokenViewModel()
                {
                    AccessTokenToken = accessToken,
                    RefreshToken = RT,
                    ExpiredAt = _refreshHandler.GetRefreshTokenByUserID(user.UserId).ExpiredTime
                };
            }
            return new TokenViewModel()
            {
                AccessTokenToken = accessToken,
                RefreshToken = GenerateRefreshToken(user),
                ExpiredAt = _refreshHandler.GetRefreshTokenByUserID(user.UserId).ExpiredTime
            };
        }
        #endregion

        #region GenerateRefreshToken
        // Hàm tạo refresh token chỉ chứa ký tự chữ cái và mã hóa Base64
        [NonAction]
        public string GenerateRefreshToken(User user)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const int randomStringLength = 32; // Chọn độ dài phù hợp

            var randomBytes = new byte[randomStringLength];
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(randomBytes);
            }

            var chars = new char[randomStringLength];
            for (int i = 0; i < randomStringLength; i++)
            {
                chars[i] = validChars[randomBytes[i] % validChars.Length];
            }

            string randomString = new string(chars);
            string base64String = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(randomString));

            var refreshTokenEntity = new Token
            {
                UserId = user.UserId,
                AccessToken = new Random().Next().ToString(),
                RefreshToken = base64String.TrimEnd('='),
                ExpiredTime = DateTime.Now.AddDays(7),
                Status = 1
            };

            _refreshHandler.GenerateRefreshToken(refreshTokenEntity);
            return base64String.TrimEnd('=');
        }
        #endregion




        #region RefreshAccessToken
        /// <summary>
        /// Refresh Token
        /// </summary>
        /// <returns>New Token and Refresh Token</returns>
        [HttpPost("refresh-access-token")]
        public async Task<ActionResult> RefreshAccessToken(RefreshTokenModel token)
        {
            try
            {
                // Kiểm tra RefreshToken trong cơ sở dữ liệu
                var storedToken = _refreshHandler.GetRefreshToken(token.RefreshToken);
                if (storedToken == null)
                {
                    return Ok(new ResultModel
                    {
                        IsSuccess = false,
                        Message = "RefreshToken does not exist"
                    });
                }

                // Kiểm tra RefreshToken có bị thu hồi không
                if (storedToken.Status == 2)
                {
                    return Ok(new ResultModel
                    {
                        IsSuccess = false,
                        Message = "RefreshToken has been revoked"
                    });
                }

                // Kiểm tra RefreshToken có hết hạn không
                if (storedToken.ExpiredTime < DateTime.UtcNow)
                {
                    return Ok(new ResultModel
                    {
                        IsSuccess = false,
                        Message = "RefreshToken has expired"
                    });
                }

                // Lấy thông tin người dùng từ UserId trong RefreshToken
                var user = _userServices.GetUserById(storedToken.UserId);

                // Tạo mới AccessToken
                var newAccessToken = GenerateToken(user, token.RefreshToken);

                return Ok(new ResultModel
                {
                    IsSuccess = true,
                    Message = "Successfully refreshed AccessToken",
                    Data = newAccessToken
                });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                return BadRequest(new ResultModel
                {
                    IsSuccess = false,
                    Code = 500,
                    Message = "Something went wrong"
                });
            }
        }
        #endregion

        #region Login
        /// <summary>
        /// Login into system
        /// </summary>
        /// <returns>Acces Token and Refresh Token</returns>
        [HttpPost]
        [Route("login")]
        public IActionResult Login(string userName, string password)
        {
            var user = _userServices.GetUserByUserName(userName);
            if (user != null && user.Status == 1)
            {
                // Assuming PasswordHasher is a utility class you have for hashing and verifying passwords
                var hashedInputPasswordString = PasswordHasher.HashPassword(password);

                if (hashedInputPasswordString == user.Password)
                {
                    // Convert user.Id to string using .ToString()
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

                    var identity = new ClaimsIdentity(claims, "Bearer");
                    var principal = new ClaimsPrincipal(identity);

                    _refreshHandler.ResetRefreshToken();
                    var token = GenerateToken(user, null);
                    return Ok(token);
                }
            }
            return BadRequest(new ResultModel
            {
                IsSuccess = false,
                Message = "Status Code:401 Unauthorized",
                Data = null
            });
        }


        #endregion

        #region Logout
        [HttpPost]
        [Route("logout")]
        public IActionResult Logout()
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"];
                token = token.Split(' ')[1];
                var tokenHandler = new JwtSecurityTokenHandler();
                var TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("c2VydmVwZXJmZWN0bHljaGVlc2VxdWlja2NvYWNoY29sbGVjdHNsb3Bld2lzZWNhbWU=")),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = false
                };
                SecurityToken validatedToken;
                var claimsPrincipal = tokenHandler.ValidateToken(token, TokenValidationParameters, out validatedToken);
                var userIdClaim = claimsPrincipal.FindFirst("UserId");
                var _refreshToken = _refreshHandler.GetRefreshTokenByUserID(userIdClaim.Value);
                _refreshHandler.UpdateRefreshToken(_refreshToken);
                _refreshHandler.ResetRefreshToken();
                if (HttpContext.Request.Headers.ContainsKey("Authorization"))
                {
                    HttpContext.Request.Headers.Remove("Authorization");
                }
                return Ok(new ResultModel
                {
                    IsSuccess = true,
                    Message = "Logout succesfully!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultModel
                {
                    IsSuccess = false,
                    Message = "Something go wrong" + ex.Message
                });
            }
        }
        #endregion

        #region Who Am I
        /// <summary>
        /// Check infor of user
        /// </summary>
        /// <returns>Infor of user</returns>
        [HttpGet("me")]
        public IActionResult WhoAmI()
        {
            // Kiểm tra xem người dùng đã được xác thực chưa
            if (User.Identity.IsAuthenticated)
            {
                // Lấy thông tin về người dùng từ claims
                var userIdClaim = User.FindFirst("UserId");
                var userNameClaim = User.FindFirst("UserName");
                var userEmailClaim = User.FindFirst("Email");
                var userRoleClaim = User.FindFirst("Role");

                // Kiểm tra xem các claim có tồn tại không
                if (userIdClaim != null && userNameClaim != null && userEmailClaim != null && userRoleClaim != null)
                {
                    var userId = userIdClaim.Value;
                    var userName = userNameClaim.Value;
                    var userEmail = userEmailClaim.Value;
                    var userRole = userRoleClaim.Value;

                    // Trả về thông tin của người dùng cùng với token
                    var user = new User
                    {
                        UserId = userId,
                        Name = userName,
                        Email = userEmail,
                        Role = int.Parse(userRole)
                    };

                    // Tạo token JWT cho người dùng


                    // Trả về thông tin của người dùng cùng với token
                    return Ok(new { UserId = userId, UserName = userName, Email = userEmail, Role = userRole });
                }
                else
                {
                    // Nếu thiếu thông tin trong claims, trả về lỗi 401 Unauthorized
                    return Unauthorized(new { Message = "Missing user information in claims" });
                }
            }
            else
            {
                // Trả về lỗi 401 Unauthorized nếu người dùng chưa được xác thực
                return Unauthorized();
            }
        }
        #endregion
    }
}
