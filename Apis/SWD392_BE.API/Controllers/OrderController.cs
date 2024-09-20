using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.ViewModels.FoodModel;
using SWD392_BE.Repositories.ViewModels.ResultModel;
using SWD392_BE.Services.Interfaces;
using SWD392_BE.Services.Services;
using System.Security.Claims;

namespace SWD392_BE.API.Controllers
{
    [Route("api/v1/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;

        public OrderController(IOrderService orderService, IUserService userService)
        {
            _orderService = orderService;
            _userService = userService;
        }

        #region Get All orders
        /// <summary>
        /// Get list of order by filter
        /// </summary>
        /// <returns>A list of orders</returns>
        [HttpGet]
        public async Task<IActionResult> GetOrders(string? userId, string? userName, DateTime? createdDate, int? status, string? storeName, string? sessionId, string? campusName, string? areaName)
        {
            var result = await _orderService.getOrders(userId, userName, createdDate, status, storeName, sessionId, campusName, areaName);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        #endregion

        #region Get Total Order Amount
        /// <summary>
        /// Get the total order amount between specified dates
        /// </summary>
        /// <param name="startDate">The start date for the calculation.</param>
        /// <param name="endDate">The end date for the calculation.</param>
        /// <returns>The total order amount</returns>

        [HttpGet("totalInCome")]
        public async Task<IActionResult> GetTotalOrderAmount(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                return BadRequest(new { message = "Start date must be less than or equal end date." });
            }

            var result = await _orderService.getTotalOrderAmount(startDate, endDate);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        #endregion

        #region Create Order
        /// <summary>
        /// Create a new order with food items and quantities.
        /// </summary>
        /// <param name="foodItems">A list of food items and their quantities.</param>
        /// <returns>The result of the order creation process.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] List<FoodItemModel> foodItems)
        {
            if (foodItems == null || !foodItems.Any())
            {
                return BadRequest(new { message = "Food items list cannot be empty." });
            }

            // Convert the incoming model to the expected tuple format for the service layer
            var orderItems = foodItems.Select(fi => (fi.FoodId, fi.Quantity, fi.Note)).ToList();

            var result = await _orderService.CreateOrderAsync(orderItems);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        #endregion

        #region Update Order Status
        [HttpPut("{orderId}")]
        public async Task<IActionResult> UpdateOrderStatus(string orderId, int status)
        {
            var currentUser = HttpContext.User;
            var result = await _orderService.updateOrderStatus(orderId, status, currentUser);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        #endregion

        #region Update All Order Statuses
        /// <summary>
        /// Update the status of all orders 3 (wait) to 1 (cancel)
        /// </summary>
        /// <returns>The result of the status update process.</returns>
        [HttpPut("AllStatuses")]
        public async Task<IActionResult> UpdateAllStatuses()
        {
            var result = await _orderService.updateAllStatuses();

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        #endregion
    }
}
