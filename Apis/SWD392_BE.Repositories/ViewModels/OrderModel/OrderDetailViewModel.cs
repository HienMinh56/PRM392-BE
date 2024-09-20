using SWD392_BE.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.ViewModels.OrderModel
{
    public class OrderDetailViewModel
    {
        public string OrderId { get; set; }
        public string FoodId { get; set; }
        public string FoodTitle { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public string? Note{ get; set; }
    }
}
