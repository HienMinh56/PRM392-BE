using SWD392_BE.Repositories.ViewModels.PaymentModel;
using SWD392_BE.Repositories.ViewModels.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Services.Interfaces
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentRequest model, string ipAddress);
        bool ValidateSignature(string inputHash, SortedList<string, string> responseData);
        Task<ResultModel> ProcessPaymentResponse(SortedList<string, string> responseData);
    }
}
