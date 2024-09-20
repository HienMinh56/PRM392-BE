using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.ViewModels.PaymentModel
{
    public class VnPayPaymentRequest
    {
        public string UserId { get; set; }
        public decimal Amount { get; set; }
    }
}
