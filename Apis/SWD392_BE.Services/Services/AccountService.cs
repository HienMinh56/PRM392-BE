using AutoMapper;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MimeKit;
using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.Interfaces;
using SWD392_BE.Repositories.ViewModels.ResultModel;
using SWD392_BE.Repositories.ViewModels.UserModel;
using SWD392_BE.Services.Interfaces;
using SWD392_BE.Services.Sercurity;
using System.Net;
using System.Security.Claims;

namespace SWD392_BE.Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepo;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AccountService(IAccountRepository accountRepo, IConfiguration configuration, IMapper mapper)
        {
            _accountRepo = accountRepo;
            _configuration = configuration;
            _mapper = mapper;
        }


        public async Task<ResultModel> AddNewUser(RegisterReqModel model, ClaimsPrincipal userCreate)
        {
            ResultModel result = new ResultModel();
            try
            {
                // Check if passwords match
                if (model.Password != model.ConfirmPassword)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Passwords do not match";
                    return result;
                }

                // Check if user already exists
                var existingUser = _accountRepo.Get(u => u.UserName == model.UserName);
                if (existingUser != null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "User already exists";
                    return result;
                }
                // Check if email already exists
                var existingEmail = _accountRepo.Get(u => u.Email == model.Email);
                if (existingEmail != null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Email already exists";
                    return result;
                }
                // Check if phone already exists
                var existingPhone = _accountRepo.Get(u => u.Phone == model.Phone);
                if (existingPhone != null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Phone already exists";
                    return result;
                }

                // Map the request model to the user entity
                var user = _mapper.Map<User>(model);

                // Generate the next user ID
                user.UserId = await _accountRepo.GenerateNewUserId();

                // Hash the password using PasswordHasher
                user.Password = PasswordHasher.HashPassword(model.Password);

                // Set other properties (e.g., CreatedDate, Status, etc.)
                user.CreatedBy = userCreate.FindFirst("UserName")?.Value;
                user.CreatedDate = DateTime.UtcNow;
                user.Status = 1; // Assuming 1 is the default status for an active user

                // Add the user to the repository and save changes
                _accountRepo.Add(user);
                _accountRepo.SaveChanges();
                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Add New User Success";
                return result;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = e.Message;
                return result;
            }
        }


        public async Task<ResultModel> MobileRegister(CreateMobileViewModel model)
        {
            ResultModel result = new ResultModel();
            try
            {
                // Check if passwords match
                if (model.Password != model.ConfirmPassword)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Passwords do not match";
                    return result;
                }

                // Check if user already exists
                var existingUser = _accountRepo.Get(u => u.UserName == model.UserName);
                if (existingUser != null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "User already exists";
                    return result;
                }
                // Check if email already exists
                var existingEmail = _accountRepo.Get(u => u.Email == model.Email);
                if (existingEmail != null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Email already exists";
                    return result;
                }
                // Check if phone already exists
                var existingPhone = _accountRepo.Get(u => u.Phone == model.Phone);
                if (existingPhone != null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Phone already exists";
                    return result;
                }
                // Map the request model to the user entity
                var user = _mapper.Map<User>(model);

                // Generate the next user ID
                user.UserId = await _accountRepo.GenerateNewUserId();

                // Hash the password using PasswordHasher
                user.Password = PasswordHasher.HashPassword(model.Password);

                // Set other properties (e.g., CreatedDate, Status, etc.)

                user.CreatedDate = DateTime.UtcNow;
                user.Role = 2; // Set role to 2 by default
                user.Status = 1; // Assuming 1 is the default status for an active user

                // Add the user to the repository and save changes
                _accountRepo.Add(user);
                await _accountRepo.SaveChangesAsync();
                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Add New Shiper Success";
                return result;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = e.Message;
                return result;
            }
        }
        public bool IsUserValid(string email)
        {
            var user = _accountRepo.Get(u => u.Email == email && u.Status == 1);
            if (user != null)
            {
                return true;
            }
            return false;
        }
        private string GenerateRandomPassword()
        {
            // Implement your method to generate a strong random password
            // Example implementation:
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+-=";
            var random = new Random();
            var password = new string(Enumerable.Repeat(validChars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return password;
        }
        public async Task<ResultModel> SendPasswordResetEmail(string emailTo)
        {
            try
            {
                
                if (!IsUserValid(emailTo))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 400,
                        Message = "User not found or email is not confirmed."
                    };
                }
                var user = _accountRepo.Get(u => u.Email == emailTo);  
                // Generate a new random password
                var newPassword = GenerateRandomPassword();

                // Hash the new password
                user.Password = PasswordHasher.HashPassword(newPassword);

                // Update user in database
                _accountRepo.Update(user);
                await _accountRepo.SaveChangesAsync(); // Assuming async method for saving changes

                // Prepare email message
                string smtpHost = _configuration["EmailSettings:EmailHost"];
                int smtpPort = int.Parse(_configuration["EmailSettings:EmailPort"]);
                string emailUserName = _configuration["EmailSettings:EmailUserName"];
                string emailPassword = _configuration["EmailSettings:EmailPassword"];

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Dev NomNom", emailUserName));
                message.To.Add(new MailboxAddress("", emailTo));
                message.Subject = "Password Reset";

                // Body of the email containing the new password
                var builder = new BodyBuilder
                {
                    HtmlBody = $@"<p>Xin chào {user.UserName},</p>
                         <p>Cảm ơn bạn đã sử dụng ứng dụng của chúng tôi. Chúc bạn có những trải nghiệm vui vẻ và ngon miệng.</p>
                         <p>Đây là mật khẩu mới của bạn: {newPassword}</p>"
                };

                message.Body = builder.ToMessageBody();

                // Send email
                using (var client = new SmtpClient())
                {
                    client.Connect(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                    client.Authenticate(emailUserName, emailPassword);
                    client.Send(message);
                    client.Disconnect(true);
                }

                return new ResultModel
                {
                    IsSuccess = true,
                    Code = 200,
                    Message = "Email sent successfully."
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = 500,
                    Message = $"Failed to send email: {ex.Message}"
                };
            }
        }

    }
}
