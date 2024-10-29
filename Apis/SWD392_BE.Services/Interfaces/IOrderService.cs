using SWD392_BE.Repositories.ViewModels.OrderModel;
using SWD392_BE.Repositories.ViewModels.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Services.Interfaces
{
    public interface IOrderService
    {
        Task<ResultModel> getOrders(string? userId, string? userName, DateTime? createdDate,
                                         int? status, string? storeName, string? sessionId,
                                         string? campusName, string? areaName);
        Task<ResultModel> getTotalOrderAmount(DateTime startDate, DateTime endDate);
        Task<ResultModel> getOrderAmountPerDayInMonth(int year, int month);
        Task<ResultModel> getOrderAmountPerWeekInMonth(int year, int month);
        Task<ResultModel> getOrderAmountPerMonthInYear(int year);
        Task<ResultModel> CreateOrderAsync(List<(string FoodId, int Quantity, string Note)> foodItems, string voucherCode = null);
        Task<ResultModel> updateOrderStatus(string orderId, int status, ClaimsPrincipal user);
        Task<ResultModel> getTotalOrderCount();
        Task<ResultModel> updateAllStatuses();
    }
}
