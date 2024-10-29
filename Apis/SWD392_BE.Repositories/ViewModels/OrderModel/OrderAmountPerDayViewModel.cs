using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.ViewModels.OrderModel
{
    public class OrderAmountPerDayViewModel
    {
        public string Day { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
