using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Net.payOS;
using Net.payOS.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sprache;
using SWD392_BE.Repositories.Interfaces;
using SWD392_BE.Repositories.Repositories;
using SWD392_BE.Repositories.ViewModels.PaymentModel;
using SWD392_BE.Repositories.ViewModels.ResultModel;
using SWD392_BE.Services.Interfaces;
using SWD392_BE.Services.Services;
using SWD392_BE.Services.Services.Payment;
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
        private readonly IPayOsServices _payOsServices;
        private readonly IOrderDetailsRepository _orderDetailsRepository;

        public PaymentController(IVnPayService vnPayService, ILogger<PaymentController> logger, IPayOsServices payOsServices, ITransactionService transactionService, IOrderRepository orderRepository, IFoodRepository foodRepository)
        {
            _vnPayService = vnPayService;
            _logger = logger;
            _payOsServices = payOsServices;
            _transactionService = transactionService;
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

        #region Check infor to Transaction - PayOs
        /// Check infor to Transaction
        /// </summary>
        /// <returns> Infor of Transaction</returns>
        [HttpGet("Check/{TransactionId}")]
        public async Task<IActionResult> CreatePaymentLink(string TransactionId)
        {
            try
            {
                TransactionId = TransactionId.ToUpper();
                if (TransactionId.StartsWith("TRANS"))
                {
                    TransactionId = TransactionId.Substring(5);
                }

                var result = await _payOsServices.GetTransactionPayment(TransactionId);
                return Ok(result);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(new PayOsResponse
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Something happen unexpectedly!"
                });
            }
        }
        #endregion

        #region Create Payment URL For vnpay
        /// <summary>
        /// Creates a URL for VN PAY
        /// </summary>
        /// <returns>Link to VN PAY</returns>
        [HttpPost("url")]
        public IActionResult CreatePaymentUrl([FromBody] PaymentRequest model)
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

        #region Create Payment URL For PayOs
        /// <summary>
        /// Create Payment URL For PayOs
        /// </summary>
        [HttpPost("payOs")]
        public async Task<IActionResult> CreatePaymentLink([FromBody] PaymentRequest model)
        {
            if (model == null)
            {
                return BadRequest("Invalid payment request.");
            }

            try
            {
                string paymentLink = await _payOsServices.CreatePaymentLink(model);
                return Ok(new { Url = paymentLink });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        #endregion

        #region Webhook
        /// <summary>
        /// Data from Webhook
        /// </summary>
        [HttpPost("hook")]
        public async Task<IActionResult> ReceiveWebhook([FromBody] WebhookType webhookBody)
        {
            try
            {
                var result = await _payOsServices.ProcessPaymentResponse(webhookBody);

                if (result.IsSuccess)
                {
                    return Ok(new { Message = "Webhook processed successfully", TransactionId = result.Code });
                }

                return BadRequest(new { Message = "Webhook processing failed.", Code = result.Code });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest(new { Message = ex.Message });
            }
        }
        #endregion
    }
}