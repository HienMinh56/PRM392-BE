using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.Interfaces;
using SWD392_BE.Repositories.Repositories;
using SWD392_BE.Repositories.ViewModels.OrderModel;
using SWD392_BE.Repositories.ViewModels.ResultModel;
using SWD392_BE.Repositories.ViewModels.UserModel;
using SWD392_BE.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Services.Services
{
    public class OrderServices : IOrderService
    {
        private readonly IOrderRepository _order;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderServices(IOrderRepository order, IHttpContextAccessor httpContextAccessor)
        {
            _order = order;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResultModel> getOrders(string? userId, string? userName, DateTime? createdDate, int? status, string? storeName, string? sessionId, string? campusName, string? areaName)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include
            };

            var result = new ResultModel();
            try
            {
                var orders = _order.GetOrders();

                if (!string.IsNullOrEmpty(userId))
                {
                    orders = orders.Where(o => o.UserId.ToLower() == userId.ToLower());
                }

                if (!string.IsNullOrEmpty(userName))
                {
                    orders = orders.Where(o => o.User.Name.ToLower().Contains(userName.ToLower()));
                }

                if (createdDate.HasValue)
                {
                    orders = orders.Where(o => o.CreatedDate.HasValue && o.CreatedDate.Value.Date == createdDate.Value.Date);
                }

                if (status.HasValue)
                {
                    orders = orders.Where(o => o.Status == status.Value);
                }

                if (!string.IsNullOrEmpty(storeName))
                {
                    orders = orders.Where(o => o.Store.Name.ToLower() == storeName.ToLower());
                }

                if (!string.IsNullOrEmpty(sessionId))
                {
                    orders = orders.Where(o => o.SessionId == sessionId);
                }

                if (!string.IsNullOrEmpty(campusName))
                {
                    orders = orders.Where(o => o.User.Campus.Name.ToLower() == campusName.ToLower());
                }

                if (!string.IsNullOrEmpty(areaName))
                {
                    orders = orders.Where(o => o.User.Campus.Area.Name.ToLower() == areaName.ToLower());
                }

                var totalCount = await orders.CountAsync();
                if (!orders.Any())
                {
                    result.Message = "Data not found";
                    result.IsSuccess = false;
                    result.Code = 404;
                }
                else
                {
                    var orderViewModels = await orders.Select(o => new OrderListViewModel
                    {
                        OrderId = o.OrderId,
                        Name = o.User.Name,
                        SessionId = o.SessionId,
                        Price = o.Price,
                        Quantity = o.Quantity,
                        StoreName = o.Store.Name,
                        TransationId = o.TransactionId,
                        VoucherCode = o.VoucherId != null ? o.Voucher.VoucherCode : "null",
                        Status = o.Status,
                        CreatedTime = o.CreatedTime,
                        CreatedDate = o.CreatedDate,
                        CampusName = o.User.Campus.Name,
                        AreaName = o.User.Campus.Area.Name,
                        ModifiedBy = o.ModifiedBy,
                        ModifiedDate = o.ModifiedDate,
                    }).ToListAsync();

                    result.Data = orderViewModels;
                    result.Message = "Success";
                    result.IsSuccess = true;
                    result.Code = 200;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.IsSuccess = false;
                result.Code = 500;
            }
            return result;
        }

        public async Task<ResultModel> getTotalOrderAmount(DateTime startDate, DateTime endDate)
        {
            var result = new ResultModel();
            try
            {
                var totalAmount = await _order.GetOrders()
                                                        .Where(o => o.CreatedDate.HasValue && o.CreatedDate.Value.Date >= startDate.Date && o.CreatedDate.Value.Date <= endDate.Date)
                                                        .SumAsync(o => o.Price * 1000);

                decimal percentageDifference = 0;
                decimal previousTotalAmount = 0;

                if (startDate.Date == endDate.Date)
                {
                    DateTime previousDate = startDate.AddDays(-1);

                    previousTotalAmount = await _order.GetOrders()
                                                      .Where(o => o.CreatedDate.HasValue
                                                              && o.CreatedDate.Value.Date == previousDate.Date)
                                                      .SumAsync(o => o.Price * 1000);

                    decimal combinedAmount = totalAmount + previousTotalAmount;

                    if (combinedAmount != 0)
                    {
                        decimal percentageToday = (totalAmount / combinedAmount) * 100;
                        decimal percentageYesterday = (previousTotalAmount / combinedAmount) * 100;
                        percentageDifference = Math.Round(percentageToday - percentageYesterday, 2);
                    }
                }

                result.Data = new
                {
                    totalAmount,
                    previousTotalAmount,
                    percentageDifference = $"{percentageDifference}%"
                };
                result.Message = "Success";
                result.IsSuccess = true;
                result.Code = 200;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.IsSuccess = false;
                result.Code = 500;
            }
            return result;
        }

        public async Task<ResultModel> getOrderAmountPerDayInMonth(int year, int month)
        {
            var result = new ResultModel();
            try
            {
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1).AddTicks(-1);

                var orders = await _order.GetOrdersByDateRange(startDate, endDate);

                var data = orders
                    .Where(o => o.CreatedDate.HasValue)
                    .GroupBy(o => o.CreatedDate.Value.Date)
                    .Select(g => new OrderAmountPerDayViewModel
                    {
                        Day = g.Key.ToString("yyyy-MM-dd"),
                        TotalAmount = g.Sum(o => o.Price) * 1000
                    })
                    .ToList();

                result.Data = data;
                result.Message = "Success";
                result.IsSuccess = true;
                result.Code = 200;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.IsSuccess = false;
                result.Code = 500;
            }
            return result;
        }

        public async Task<ResultModel> CreateOrderAsync(List<(string FoodId, int Quantity, string Note)> foodItems, string voucherCode)
        {
            var result = new ResultModel();
            try
            {
                var userNameClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserName");
                var userName = userNameClaim?.Value;
                var userIdClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");
                var userId = userIdClaim.Value;

                var order = await _order.CreateOrder(foodItems, userId, userName, voucherCode);
                if (order != null)
                {
                    result.Data = order;
                    result.Message = "Order created successfully";
                    result.IsSuccess = true;
                    result.Code = 200;
                }
                else
                {
                    result.Data = null;
                    result.Message = "Failed to create order";
                    result.IsSuccess = false;
                    result.Code = 400;
                }
            }
            catch (Exception ex)
            {
                result.Message = $"An error occurred: {ex.Message}";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<ResultModel> getOrderAmountPerWeekInMonth(int year, int month)
        {
            var result = new ResultModel();
            try
            {
                var data = await _order.GetOrderAmountPerWeekInMonth(year, month);

                var updatedData = data.Select(d => new OrderAmountPerWeekViewModel
                {
                    WeekNumber = d.WeekNumber, 
                    TotalAmount = d.TotalAmount * 1000
                }).ToList();

                result.Data = updatedData;
                result.Message = "Success";
                result.IsSuccess = true;
                result.Code = 200;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.IsSuccess = false;
                result.Code = 500;
            }
            return result;
        }

        public async Task<ResultModel> getOrderAmountPerMonthInYear(int year)
        {
            var result = new ResultModel();
            try
            {
                var data = await _order.GetOrderAmountPerMonthInYear(year);

                var updatedData = data.Select(d => new OrderAmountPerMonthViewModel
                {
                    Month = d.Month,
                    TotalAmount = d.TotalAmount * 1000
                }).ToList();

                result.Data = updatedData;
                result.Message = "Success";
                result.IsSuccess = true;
                result.Code = 200;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.IsSuccess = false;
                result.Code = 500;
            }
            return result;
        }

        public Task<ResultModel> updateOrderStatus(string orderId, int status, ClaimsPrincipal user)
        {
            var order = _order.Get(o => o.OrderId == orderId);
            if (order == null)
            {
                return Task.FromResult(new ResultModel
                {
                    Message = "Order not found",
                    IsSuccess = false,
                    Code = 404
                });
            }
            else
            {
                   order.Status = status;
                order.ModifiedBy = user.FindFirst("UserName")?.Value;
                order.ModifiedDate = DateTime.Now;
                _order.Update(order);
                _order.SaveChanges();
                return Task.FromResult(new ResultModel
                {
                    Message = "Order status updated successfully",
                    IsSuccess = true,
                    Code = 200
                });
            }
        }

        public async Task<ResultModel> getTotalOrderCount()
        {
            var result = new ResultModel();
            try
            {
                var totalOrderCount = await _order.GetOrders().CountAsync();

                result.Data = totalOrderCount;
                result.Message = "Success";
                result.IsSuccess = true;
                result.Code = 200;
            }
            catch (Exception ex)
            {
                result.Message = $"An error occurred: {ex.Message}";
                result.IsSuccess = false;
                result.Code = 500;
            }
            return result;
        }

        public async Task<ResultModel> updateAllStatuses()
        {
            var result = new ResultModel();
            try
            {
                var orders = await _order.GetOrders()
                                          .Where(o => o.Status == 3)
                                          .ToListAsync();

                foreach (var order in orders)
                {
                    order.Status = 1;
                }

                await _order.SaveChangesAsync();

                result.Message = "Updated statuses successfully";
                result.IsSuccess = true;
                result.Code = 200;
            }
            catch (Exception ex)
            {
                result.Message = $"An error occurred: {ex.Message}";
                result.IsSuccess = false;
                result.Code = 500;
            }
            return result;
        }
    }
}
