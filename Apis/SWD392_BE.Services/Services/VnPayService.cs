using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.Interfaces;
using SWD392_BE.Repositories.ViewModels.PaymentModel;
using SWD392_BE.Repositories.ViewModels.ResultModel;
using SWD392_BE.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWD392_BE.Services.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly ITransactionRepository _transactionRepository;

        public VnPayService(IUserService userService, IConfiguration configuration, ITransactionRepository transactionRepository)
        {
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
            var numericPart = int.Parse(latestId.Substring(5)) + 1;
            return $"TRANS{numericPart:D3}";
        }

        public string CreatePaymentUrl(VnPayPaymentRequest model, string ipAddress)
        {
            var transaction = new Transaction
            {
                TransactionId = GenerateTransactionId(),
                UserId = model.UserId,
                Type = 2, // recharge
                Amount = (int)(model.Amount),
                Status = 2, // Pending
                CreatedDate = DateTime.Now,
                CreatTime = DateTime.Now.TimeOfDay
            };
            _transactionRepository.Add(transaction);
            _transactionRepository.SaveChanges();

            string txnRef = transaction.TransactionId.ToString();

            var vnPay = new VnPayLibraryService();
            vnPay.AddRequestData("vnp_Version", VnPayLibraryService.VERSION);
            vnPay.AddRequestData("vnp_Command", "pay");
            vnPay.AddRequestData("vnp_TmnCode", _configuration["VNPAY:TmnCode"]);
            vnPay.AddRequestData("vnp_Amount", ((int)(model.Amount * 100)).ToString());
            vnPay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnPay.AddRequestData("vnp_CurrCode", "VND");
            vnPay.AddRequestData("vnp_IpAddr", ipAddress);
            vnPay.AddRequestData("vnp_Locale", "vn");
            vnPay.AddRequestData("vnp_OrderInfo", Uri.EscapeDataString($"Deposit {model.Amount} into wallet with transaction id: {txnRef}"));
            vnPay.AddRequestData("vnp_OrderType", "others");
            vnPay.AddRequestData("vnp_ReturnUrl", _configuration["VNPAY:ReturnUrl"]);
            vnPay.AddRequestData("vnp_TxnRef", txnRef);
            vnPay.AddRequestData("vnp_BankCode", "NCB");

            string paymentUrl = vnPay.CreateRequestUrl(_configuration["VNPAY:Url"], _configuration["VNPAY:HashSecret"]);

            return paymentUrl;
        }

        public bool ValidateSignature(string inputHash, SortedList<string, string> responseData)
        {
            var vnPay = new VnPayLibraryService();
            foreach (var kv in responseData)
            {
                vnPay.AddResponseData(kv.Key, kv.Value);
            }
            return vnPay.ValidateSignature(inputHash, _configuration["VNPAY:HashSecret"]);
        }

        public async Task<ResultModel> ProcessPaymentResponse(SortedList<string, string> responseData)
        {
            string vnp_ResponseCode = responseData["vnp_ResponseCode"];
            int responseCode;
            if (!int.TryParse(vnp_ResponseCode, out responseCode))
            {
                responseCode = -1;
            }

            string txnRef = responseData["vnp_TxnRef"];
            var transaction = _transactionRepository.GetByTransactionId(txnRef);

            if (transaction != null && responseCode == 0)
            {
                transaction.Status = 1; // Success
                _transactionRepository.Update(transaction);
                await _transactionRepository.SaveChangesAsync();

                var user = _userService.GetUserById(transaction.UserId);
                if (user != null)
                {
                    var result = await _userService.UpdateUserBalance(transaction.UserId, transaction.Amount/1000);
                    result.Code = responseCode;
                    return result;
                }
            }
            else
            {
                if (transaction != null)
                {
                    transaction.Status = 3; // Failed
                    _transactionRepository.Update(transaction);
                    await _transactionRepository.SaveChangesAsync();
                }
            }

            return new ResultModel { IsSuccess = false, Code = responseCode, Message = "Payment failed" };
        }
    }
}