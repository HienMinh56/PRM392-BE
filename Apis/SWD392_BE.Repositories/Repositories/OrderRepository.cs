using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Math.EC.Rfc7748;
using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.Interfaces;
using SWD392_BE.Repositories.ViewModels.OrderModel;
using SWD392_BE.Repositories.ViewModels.ResultModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly CampusFoodSystemContext _dbContext;
        public OrderRepository(CampusFoodSystemContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Order> GetOrders()
        {
            return _dbContext.Orders
                .Include(o => o.User)
                    .ThenInclude(u => u.Campus)
                        .ThenInclude(c => c.Area)
                .Include(o => o.Store)
                .Include(o => o.Session)
                .AsQueryable();
        }

        public IQueryable<Order> GetQuery() => _dbContext.Orders.AsQueryable();

        public async Task<List<Order>> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            return await _dbContext.Orders
                .Include(o => o.User)
                    .ThenInclude(u => u.Campus)
                        .ThenInclude(c => c.Area)
                .Include(o => o.Store)
                .Include(o => o.Session)
                .Where(o => o.CreatedDate >= startDate && o.CreatedDate <= endDate)
                .ToListAsync();
        }

        public async Task<Order> CreateOrder(List<(string foodId, int quantity, string note)> foodItems, string userId, string userName, string voucherCode)
        {
            var newOrderId = await GenerateNewOrderIdAsync();
            var newTransactionId = await GenerateNewTransactionIdAsync();
            var session = await GetCurrentSessionAsync();

            var order = new Order
            {
                OrderId = newOrderId,
                UserId = userId,
                SessionId = session.SessionId,
                CreatedDate = DateTime.Now,
                CreatedBy = userName,
                CreatedTime = DateTime.Now.TimeOfDay,
                TransactionId = newTransactionId,
                Status = 3,
            };

            int totalPrice = 0;
            foreach (var item in foodItems)
            {
                var food = await _dbContext.Foods.FirstOrDefaultAsync(f => f.FoodId == item.foodId);
                if (food == null)
                {
                    throw new Exception($"Food with ID {item.foodId} not found.");
                }

                order.StoreId = food.StoreId;

                var orderDetail = new OrderDetail
                {
                    FoodId = item.foodId,
                    Quantity = item.quantity,
                    Price = food.Price * item.quantity,
                    OrderId = order.OrderId,
                    Status = 1,
                    Note = item.note
                };

                totalPrice += orderDetail.Price;
                order.OrderDetails.Add(orderDetail);
            }

            // Apply voucher if provided
            if (!string.IsNullOrEmpty(voucherCode))
            {
                var voucher = await _dbContext.Vouchers.FirstOrDefaultAsync(v => v.VoucherCode == voucherCode && v.ValidityStartDate <= DateTime.Now && v.ValidityEndDate >= DateTime.Now);
                if (voucher == null)
                {
                    throw new Exception("Invalid or expired voucher.");
                }

                // Apply the discount based on the voucher type
                if (voucher.MinOrderAmount <= totalPrice)
                    totalPrice = totalPrice - (int)voucher.DiscountAmount;
                else
                {
                    throw new Exception("Min amount is not invalid");
                }
                // Ensure total price is not below zero

                order.VoucherId = voucher.VoucherId;  // Save the voucher code with the order
            }

            order.Price = totalPrice;
            order.Quantity = foodItems.Sum(item => item.quantity);

            var newTransaction = new Transaction
            {
                TransactionId = newTransactionId,
                UserId = userId,
                Type = 1,
                Status = 1,
                Amount = totalPrice,
                CreatedDate = DateTime.Now,
                CreatTime = DateTime.Now.TimeOfDay
            };

            _dbContext.Transactions.Add(newTransaction);
            await _dbContext.SaveChangesAsync();

            try
            {
                await _dbContext.Orders.AddAsync(order);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException;
                Console.WriteLine(innerException?.Message);
                throw new Exception($"An error occurred while creating the order: {innerException?.Message}", ex);
            }

            return order;
        }


        private async Task<string> GenerateNewOrderIdAsync()
        {
            try
            {
                var lastOrder = await _dbContext.Orders.OrderByDescending(o => o.Id).FirstOrDefaultAsync();
                var lastOrderIdNumber = lastOrder != null ? lastOrder.Id : 0;

                // Tạo mã ORDERID dựa trên số tự tăng Id của đơn hàng
                return $"ORDER{lastOrderIdNumber + 1:D6}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while generating new order ID: {ex.Message}");
                throw; // Ném ngoại lệ để xử lý ở lớp gọi
            }
        }

        private async Task<Session> GetCurrentSessionAsync()
        {
            var currentTime = DateTime.Now.TimeOfDay;
            var session = await _dbContext.Sessions
                 .Where(s => currentTime >= s.StartTime && currentTime <= s.EndTime)
                 .FirstOrDefaultAsync();
            if (session == null)
            {
                throw new Exception("No session available for the current time.");
            }
            return session;
        }

        private async Task<string> GenerateNewTransactionIdAsync()
        {
            var latestTransaction = await _dbContext.Transactions.OrderByDescending(t => t.TransactionId).FirstOrDefaultAsync();
            var numericPart = latestTransaction != null ? int.Parse(latestTransaction.TransactionId.Substring(5)) + 1 : 1;
            return $"TRANS{numericPart:D3}";
        }

        public async Task<List<OrderAmountPerMonthViewModel>> GetOrderAmountPerMonthInYear(int year)
        {
            var orders = await _dbContext.Orders
                .Where(o => o.CreatedDate.HasValue && o.CreatedDate.Value.Year == year)
                .ToListAsync();

            var data = orders
                .GroupBy(o => o.CreatedDate.Value.Month)
                .Select(g => new OrderAmountPerMonthViewModel
                {
                    Month = g.Key.ToString("00"),
                    TotalAmount = g.Sum(o => o.Price)
                })
                .ToList();

            return data;
        }

        public async Task<List<OrderAmountPerWeekViewModel>> GetOrderAmountPerWeekInMonth(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var orders = await _dbContext.Orders
                .Where(o => o.CreatedDate.HasValue && o.CreatedDate.Value >= startDate && o.CreatedDate.Value <= endDate)
                .ToListAsync();

            var weekNumber = 1;
            var weeks = new List<OrderAmountPerWeekViewModel>();

            var groupedOrders = orders
                .GroupBy(o => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(o.CreatedDate.Value, CalendarWeekRule.FirstDay, DayOfWeek.Monday))
                .OrderBy(g => g.Key);

            foreach (var group in groupedOrders)
            {
                weeks.Add(new OrderAmountPerWeekViewModel
                {
                    WeekNumber = $"Week {weekNumber}",
                    TotalAmount = group.Sum(o => o.Price)
                });
                weekNumber++;
            }

            return weeks;
        }
    }
}
