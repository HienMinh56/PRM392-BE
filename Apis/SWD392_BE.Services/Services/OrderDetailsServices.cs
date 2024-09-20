using SWD392_BE.Repositories.Interfaces;
using SWD392_BE.Repositories.Repositories;
using SWD392_BE.Repositories.ViewModels.OrderModel;
using SWD392_BE.Repositories.ViewModels.ResultModel;
using SWD392_BE.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Services.Services
{
    public class OrderDetailsServices : IOrderDetailsServices
    {
        private readonly IOrderDetailsRepository _orderDetails;
        private readonly IFoodRepository _food;

        public OrderDetailsServices(IOrderDetailsRepository orderDetails, IFoodRepository food)
        {
            _orderDetails = orderDetails;
            _food = food;
        }
        public async Task<ResultModel> GetOrderDetails(string orderId)
        {
            var result = new ResultModel();
            var orderDetails = _orderDetails.GetList(o => o.OrderId == orderId);

            if (orderDetails == null || !orderDetails.Any())
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = "Order details not found";
                return result;
            }

            var orderDetailDtos = new List<OrderDetailViewModel>();

            foreach (var orderDetail in orderDetails)
            {
                var food = await _food.GetAsync(f => f.FoodId == orderDetail.FoodId);

                var orderDetailDto = new OrderDetailViewModel
                {
                    OrderId = orderDetail.OrderId,
                    FoodId = orderDetail.FoodId,
                    FoodTitle = food?.Title,
                    Quantity = orderDetail.Quantity,
                    Price = orderDetail.Price,
                    Image = food?.Image,
                    Note = orderDetail.Note
                };

                orderDetailDtos.Add(orderDetailDto);
            }

            result.IsSuccess = true;
            result.Code = 200;
            result.Message = "Get order details successfully";
            result.Data = orderDetailDtos;

            return result;
        }
    }
}