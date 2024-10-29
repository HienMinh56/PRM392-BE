using Microsoft.AspNetCore.Mvc;
using SWD392_BE.Repositories.ViewModels.ResultModel;
using SWD392_BE.Services.Interfaces;
using SWD392_BE.Services.Services;

namespace SWD392_BE.API.Controllers
{
    [Route("api/v1/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;

        public DashboardController(IOrderService orderService, IUserService userService)
        {
            _orderService = orderService;
            _userService = userService;
        }

        #region Get data for chart by day, week, month
        /// <summary>
        /// Get data for chart by day, week, month
        /// </summary>
        /// <param name="type">The type of data to retrieve: day, week, month</param>
        /// <param name="year">The year for the calculation </param>
        /// <param name="month">The month for the calculation (required for day and week types)</param>
        /// <returns>Amount 7 day in week, all week in month, all month in year</returns>
        [HttpGet("time")]
        public async Task<IActionResult> GetOrderAmounts(string type, int year, int month = 0)
        {
            ResultModel result;

            switch (type.ToLower())
            {
                case "day":
                    if (month == 0) return BadRequest(new { message = "Month is required for type 'day'." });
                    result = await _orderService.getOrderAmountPerDayInMonth(year, month);
                    break;

                case "week":
                    if (month == 0) return BadRequest(new { message = "Month is required for type 'week'." });
                    result = await _orderService.getOrderAmountPerWeekInMonth(year, month);
                    break;

                case "month":
                    result = await _orderService.getOrderAmountPerMonthInYear(year);
                    break;

                default:
                    return BadRequest(new { message = "Invalid type. Valid types are 'day', 'week', 'month'." });
            }

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        #endregion

        #region Get Total Order vs User
        /// <summary>
        /// Get the total Order vs User in the system.
        /// </summary>
        /// <returns>The total orders and user.</returns>
        [HttpGet("total")]
        public async Task<IActionResult> GetTotalOrderAndUserCount()
        {
            var orderResult = await _orderService.getTotalOrderCount();
            var userCount = await _userService.GetTotalUserCountAsync();

            if (orderResult.IsSuccess)
            {
                var result = new
                {
                    TotalOrders = orderResult.Data,
                    TotalUsers = userCount
                };
                return Ok(new ResultModel
                {
                    Data = result,
                    IsSuccess = true,
                    Message = "Success",
                    Code = 200
                });
            }
            else
            {
                return BadRequest(orderResult);
            }
        }
        #endregion
    }
}
