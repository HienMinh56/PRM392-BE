using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWD392_BE.Repositories.Helper;
using SWD392_BE.Repositories.ViewModels.ResultModel;
using SWD392_BE.Repositories.ViewModels.UserModel;
using SWD392_BE.Services.Interfaces;
using SWD392_BE.Services.Services;
using System.Net.Mail;
using System.Net;
using MailKit.Net.Smtp;
using MimeKit;
using static System.Runtime.InteropServices.JavaScript.JSType;
using SWD392_BE.Repositories.ViewModels.ResetPasswordModel;
using Microsoft.Extensions.Configuration;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using MailKit.Security;

namespace SWD392_BE.API.Controllers
{
    [Route("api/v1/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public AccountController(IAccountService accountService,IUserService userService,IConfiguration configuration)
        {
            _accountService = accountService;
            _userService = userService;
            _configuration = configuration;
        }

        #region Add a new user
        /// <summary>
        /// Add a new user by admin
        /// </summary>
        /// <returns>Status of action</returns>
        [HttpPost]
        public async Task<ActionResult<ResultModel>> AddNewUser(RegisterReqModel model)
        {
            try
            {
                var currentUser = HttpContext.User;
                var registerResult = await _accountService.AddNewUser(model, currentUser);
                var result = new ResultModel
                {
                    IsSuccess = true,
                    Code = 200,
                    Data = registerResult
                };
                return Ok(result); // Return 200 OK with the registration response
            }
            catch (Exception ex)
            {
                var result = new ResultModel
                {
                    IsSuccess = false,
                    Code = 500,
                    Message = ex.Message
                }; // Return 400 Bad Request with the error message
                return StatusCode(500, result);
            }
        }
        #endregion

        #region Regis Account
        /// <summary>
        /// Register a new account
        /// </summary>
        /// <returns>Status of action</returns>
        [HttpPost("mobile")]
        public async Task<ActionResult<ResultModel>> MobileRegister(CreateMobileViewModel model)
        {
            try
            {


                var mobileRegisterResult = await _accountService.MobileRegister(model);
                var result = new ResultModel
                {
                    IsSuccess = true,
                    Code = 200,
                    Data = mobileRegisterResult
                };
                return Ok(result);// Return the response from the common Register method
            }
            catch (Exception ex)
            {
                var result = new ResultModel
                {
                    IsSuccess = false,
                    Code = 500,
                    Message = ex.Message
                }; // Return 400 Bad Request with the error message
                return StatusCode(500, result);
            }
        }
        #endregion

        [HttpPost("send-reset-email")]
        public async Task<IActionResult> SendPasswordResetEmail([FromBody] string emailTo)
        {
            if (string.IsNullOrEmpty(emailTo))
            {
                return BadRequest("Email is required.");
            }

            var result = await _accountService.SendPasswordResetEmail(emailTo);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }


    }

}
