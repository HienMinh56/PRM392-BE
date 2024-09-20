using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SWD392_BE.Repositories.Interfaces;
using SWD392_BE.Repositories.ViewModels.PaymentModel;
using SWD392_BE.Repositories.ViewModels.ResultModel;
using SWD392_BE.Services.Interfaces;
using SWD392_BE.Services.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SWD392_BE.API.Controllers
{
    [Route("api/v1/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly ILogger<PaymentController> _logger;
        private readonly ITransactionService _transactionService;

        public PaymentController(IVnPayService vnPayService, ILogger<PaymentController> logger)
        {
            _vnPayService = vnPayService;
            _logger = logger;
        }

        #region VN PAY Return
        /// <summary>
        /// Handles the return from VN PAY
        /// </summary>
        /// <returns>Message from VN PAY</returns>

        [HttpGet("url")]
        public async Task<IActionResult> VnPayReturn()
        {
            try
            {
                var responseData = new SortedList<string, string>();
                foreach (var key in Request.Query.Keys)
                {
                    responseData.Add(key, Request.Query[key]);
                }

                if (responseData.TryGetValue("vnp_SecureHash", out var vnp_SecureHash))
                {
                    bool isValidSignature = _vnPayService.ValidateSignature(vnp_SecureHash, responseData);
                    if (isValidSignature)
                    {
                        var result = await _vnPayService.ProcessPaymentResponse(responseData);
                        return Ok(result);
                    }
                    else
                    {
                        _logger.LogWarning("Invalid signature for VNPAY response: {responseData}", responseData);
                        return BadRequest(new ResultModel { IsSuccess = false, Code = 97, Message = "Invalid signature" });
                    }
                }
                else
                {
                    _logger.LogWarning("Missing secure hash in VNPAY response: {responseData}", responseData);
                    return BadRequest(new ResultModel { IsSuccess = false, Code = 97, Message = "Missing secure hash" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing VN PAY return");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region Create Payment URL
        /// <summary>
        /// Creates a URL for VN PAY
        /// </summary>
        /// <returns>Link to VN PAY</returns>
        [HttpPost("url")]
        public IActionResult CreatePaymentUrl([FromBody] VnPayPaymentRequest model)
        {
            if (model == null || string.IsNullOrEmpty(model.UserId) || model.Amount <= 0)
            {
                return BadRequest("Invalid payment request model.");
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrEmpty(ipAddress))
            {
                return BadRequest("Unable to determine IP address.");
            }

            try
            {
                var paymentUrl = _vnPayService.CreatePaymentUrl(model, ipAddress);
                return Ok(new { paymentUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment URL");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion
    
    }
}