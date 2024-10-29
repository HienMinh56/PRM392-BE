using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.ViewModels.OrderModel
{
    public class GetOrderViewModel
    {
        public string OrderId { get; set; }
        public string SessionId { get; set; } = null!;
        public string TransationId { get; set; } = null!;
        public string UserName { get; set; }
        public string StoreName { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }
        public TimeSpan CreatedTime { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
