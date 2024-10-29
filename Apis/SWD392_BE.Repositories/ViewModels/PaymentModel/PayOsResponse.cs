using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.ViewModels.PaymentModel
{
    public class PayOsResponse
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public ICollection<object>? Data { get; set; }
        public string Code { get; set; }  
        public string Desc { get; set; }
    }
}
