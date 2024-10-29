using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SWD392_BE.Repositories.ViewModels.FoodModel;
using SWD392_BE.Repositories.ViewModels.PageModel;
using SWD392_BE.Repositories.ViewModels.StoreModel;
using SWD392_BE.Services.Interfaces;
using SWD392_BE.Services.Services;

namespace SWD392_BE.API.Controllers
{
    [Route("api/v1/store")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public StoreController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        #region Get store by filter
        /// <summary>
        /// Get list of stores by filter
        /// </summary>
        /// <returns>A list of stores</returns>
        [HttpGet]
        public async Task<IActionResult> GetStoresByStatusAreaAndSession([FromQuery] string? Name, [FromQuery] string? StoreId, [FromQuery] int? status, [FromQuery] string? areaName, [FromQuery] string? sessionId)
        {
            var stores = await _storeService.GetStoresByStatusAreaAndSessionAsync(Name, StoreId ,status, areaName, sessionId);
            return Ok(stores);
        }

        #endregion

        #region Search store
        /// <summary>
        /// Search store by name or phone
        /// </summary>
        /// <returns>A store</returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchStoreByNameOrPhone([FromQuery] string keyword)
        {
            var result = await _storeService.SearchStoreByNameOrPhone(keyword);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        #endregion

        #region Add store
        /// <summary>
        /// Add a new store 
        /// </summary>
        /// <returns>Status of action</returns>
        [HttpPost]
        public async Task<IActionResult> AddStore(StoreViewModel storeReq)
        {
            var currentUser = HttpContext.User;
            var result = await _storeService.AddStore(storeReq, currentUser);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        #endregion

        #region Update store status manually
        /// <summary>
        /// Manually update store statuses
        /// </summary>
        /// <returns>Status of action</returns>
        [HttpPost("status")]
        public async Task<IActionResult> UpdateStoreStatus()
        {
            await _storeService.UpdateStoreStatusAsync();
            return Ok(new { message = "Store statuses updated successfully." });
        }
        #endregion

        #region Update store
        /// <summary>
        /// Update a store
        /// </summary>
        /// <returns>Status of action</returns>
        [HttpPut("{storeId}")]
        public async Task<IActionResult> UpdateStore(string storeId, UpdateStoreViewModel storeReq)
        {
            var currentUser = HttpContext.User;
            var result = await _storeService.UpdateStoreAsync(storeId, storeReq, currentUser);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        #endregion

        #region Delete store
        /// <summary>
        /// Delete a store
        /// </summary>
        /// <returns>Status of action</returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteStore([FromBody] DeleteStoreReqModel request)
        {
            var currentUser = HttpContext.User;
            var result = await _storeService.DeleteStore(request, currentUser);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        #endregion
    }
}
