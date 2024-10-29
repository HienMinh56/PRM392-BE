using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SWD392_BE.Repositories.Interfaces;
using SWD392_BE.Repositories.ViewModels.PaymentModel;
using SWD392_BE.Repositories.ViewModels.ResultModel;
using SWD392_BE.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace SWD392_BE.Services.Services.Payment
{
    public class PayOsServices : IPayOsServices
    {
        private readonly PayOS _payOs;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly ITransactionRepository _transactionRepository;

        public PayOsServices(PayOS payOs, IUserService userService, IConfiguration configuration, ITransactionRepository transactionRepository)
        {
            _payOs = payOs;
            _userService = userService;
            _configuration = configuration;
            _transactionRepository = transactionRepository;
        }

        private string GenerateTransactionId()
        {
            var latestTransaction = _transactionRepository.GetLatestTransaction();
            if (latestTransaction == null)
            {
                return "TRANS001";
            }

            var latestId = latestTransaction.TransactionId;

            if (latestId.Length < 6)
            {
                throw new Exception("Invalid transaction ID format.");
            }

            var numericPartString = latestId.Substring(5);
            if (!int.TryParse(numericPartString, out int numericPart))
            {
                throw new Exception($"Invalid numeric part in transaction ID: {latestId}");
            }

            numericPart++; 
            return $"TRANS{numericPart:D3}"; 
        }
        public async Task<string> CreatePaymentLink(PaymentRequest model)
        {
            string txnRef = GenerateTransactionId();
            //long orderCode = long.Parse(txnRef.Substring(5));
            var transaction = new Repositories.Entities.Transaction
            {
                TransactionId = txnRef,
                UserId = model.UserId,
                Type = 2,                 // recharge 
                Amount = (int)model.Amount,
                Status = 2,               // Pending
                CreatedDate = DateTime.Now,
                CreatTime = DateTime.Now.TimeOfDay
            };
            _transactionRepository.Add(transaction);
            _transactionRepository.SaveChanges();

            long expiredAt = (long)(DateTime.UtcNow.AddMinutes(10) - new DateTime(1970, 1, 1)).TotalSeconds;

            PaymentData paymentData = new PaymentData(
                orderCode: long.Parse(txnRef.Substring(5)),
                amount: (int)model.Amount,
                description: $"Deposit {model.Amount} into wallet",
                items: new List<ItemData>(),
                cancelUrl: "https://dev.fancy.io.vn/paymen-failed/",
                returnUrl: "https://dev.fancy.io.vn/payment-page/",
                expiredAt: expiredAt
            );

            CreatePaymentResult createPaymentResult = await _payOs.createPaymentLink(paymentData);
            return createPaymentResult.checkoutUrl;
        }

        public async Task<ResultModel> ProcessPaymentResponse(WebhookType webhookBody)
        {

            WebhookData verifiedData = _payOs.verifyPaymentWebhookData(webhookBody); //xác thực data from webhook
            string responseCode = verifiedData.code;

            string orderCode = verifiedData.orderCode.ToString(); 

            string transactionId = "TRANS" + orderCode; 

            var transaction = _transactionRepository.GetByTransactionId(transactionId);

            if (transaction != null && responseCode == "00" )
            {
                transaction.Status = 1; // Success
                _transactionRepository.Update(transaction);
                await _transactionRepository.SaveChangesAsync();

                var user = _userService.GetUserById(transaction.UserId);
                if (user != null)
                {
                    var result = await _userService.UpdateUserBalance(transaction.UserId, transaction.Amount / 1000);
                    result.Code = 0;
                    return result;
                }
            }
            else
            {
                if (transaction != null)
                {
                    transaction.Status = 3; // Faild
                    _transactionRepository.Update(transaction);
                    await _transactionRepository.SaveChangesAsync();
                }
            }

            return new ResultModel { IsSuccess = false, Code = int.Parse(responseCode), Message = "Payment failed" };
        }

        public async Task<PaymentLinkInformation> GetTransactionPayment(string transactionId)
        {
            if (string.IsNullOrWhiteSpace(transactionId) || !int.TryParse(transactionId, out int orderCode))
            {
                throw new ArgumentException("Invalid order ID.");
            }

            try
            {
                PaymentLinkInformation paymentLinkInformation = await _payOs.getPaymentLinkInformation(orderCode);
                return paymentLinkInformation;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
