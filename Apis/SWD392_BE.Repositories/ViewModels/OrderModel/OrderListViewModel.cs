using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.ViewModels.OrderModel
{
    public class OrderListViewModel
    {
        public string OrderId { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string SessionId { get; set; } = null!;

        public int Price { get; set; }

        public int Quantity { get; set; }

        public string StoreName { get; set; } = null!;

        public string TransationId { get; set; } = null!;
        public string VoucherCode { get; set; } = null;

        public int Status { get; set; }
        public string CampusName { get; set; }
        public string AreaName { get; set; }

        public TimeSpan CreatedTime { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy{ get; set; }
    }
}
