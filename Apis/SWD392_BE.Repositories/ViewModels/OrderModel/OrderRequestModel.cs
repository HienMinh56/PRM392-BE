using SWD392_BE.Repositories.ViewModels.FoodModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.ViewModels.OrderModel
{
    public class OrderRequestModel
    {
        public List<FoodItemModel> FoodItems { get; set; }
        public string? VoucherCode{ get; set; }  
    }
}
