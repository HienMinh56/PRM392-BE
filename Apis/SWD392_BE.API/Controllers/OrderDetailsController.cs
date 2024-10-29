using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWD392_BE.Services.Interfaces;

namespace SWD392_BE.API.Controllers
{
    [Route("api/v1/order-details")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private readonly IOrderDetailsServices _orderDetails;

        public OrderDetailsController(IOrderDetailsServices orderDetails)
        {
            _orderDetails = orderDetails;
        }

        #region Get Order Details
        [HttpGet]
        public async Task<IActionResult> GetOrderDetails(string orderId)
        {
            var result = await _orderDetails.GetOrderDetails(orderId);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        #endregion
    }
}
