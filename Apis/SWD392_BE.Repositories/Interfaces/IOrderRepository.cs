using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.ViewModels.OrderModel;
using SWD392_BE.Repositories.ViewModels.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        IQueryable<Order> GetOrders();
        Task<List<Order>> GetOrdersByDateRange(DateTime startDate, DateTime endDate);
        Task<Order> CreateOrder(List<(string foodId, int quantity, string note)> foodItems, string userId, string userName);
        Task<List<OrderAmountPerMonthViewModel>> GetOrderAmountPerMonthInYear(int year);
        Task<List<OrderAmountPerWeekViewModel>> GetOrderAmountPerWeekInMonth(int year, int month);
    }
}