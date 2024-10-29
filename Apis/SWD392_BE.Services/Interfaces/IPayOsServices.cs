using Net.payOS.Types;
using SWD392_BE.Repositories.ViewModels.PaymentModel;
using SWD392_BE.Repositories.ViewModels.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Services.Interfaces
{
    public interface IPayOsServices
    {
        Task<string> CreatePaymentLink(PaymentRequest viewModel);      
        Task<PaymentLinkInformation> GetTransactionPayment(string orderId);
        Task<ResultModel> ProcessPaymentResponse(WebhookType webhookBody);
    }
}
