using Microsoft.AspNetCore.Mvc;
using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.ViewModels.FoodModel;
using SWD392_BE.Repositories.ViewModels.PageModel;
using SWD392_BE.Repositories.ViewModels.StoreModel;
using SWD392_BE.Repositories.ViewModels.UserModel;
using SWD392_BE.Services.Interfaces;
using SWD392_BE.Services.Services;
using System.Security.Claims;

namespace SWD392_BE.API.Controllers
{
    [Route("api/v1/food")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly IFoodService _foodService;
        private readonly ICloudStorageService _cloudStorageService;

        public FoodController(IFoodService foodService, ICloudStorageService cloudStorageService)
        {
            _foodService = foodService;
            _cloudStorageService = cloudStorageService; 
        }
        #region Get list foods
        /// <summary>
        /// Get list of foods
        /// </summary>
        /// <returns>A list of foods</returns>
        [HttpGet]
        public async Task<IActionResult> GetListFoodAsync([FromQuery] string? foodId, [FromQuery] string? storeId, [FromQuery] int? cate)
        {
            if (string.IsNullOrEmpty(storeId))
            {
                return BadRequest(new { Message = "storeId is required" });
            }

            var result = await _foodService.GetListFoodsAsync(foodId, storeId, cate);

            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return StatusCode(result.Code, result);
            }
        }
        #endregion
        #region Add foods
        /// <summary>
        /// Add new foods into store
        /// </summary>
        /// <returns>Status of action</returns>
        [HttpPost("{storeId}")]
        public async Task<IActionResult> AddFood(string storeId, [FromForm] FoodRequestModel foodRequestModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ClaimsPrincipal user = HttpContext.User;

            try
            {
                // Convert FoodRequestModel to List<FoodViewModel> if needed
                var foodViewModels = new List<FoodViewModel>()
            {
                new FoodViewModel
                {
                    Name = foodRequestModel.Name,
                    Price = foodRequestModel.Price,
                    Title = foodRequestModel.Title,
                    Description = foodRequestModel.Description,
                    Cate = foodRequestModel.Cate,
                    Image = foodRequestModel.Image
                }
            };

                // Call the addFood method from the service layer
                var result = await _foodService.addFood(storeId, foodViewModels, user, 800, 350);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
        #endregion

        #region Update Food
        /// <summary>
        /// Update a food
        /// </summary>
        /// <returns>Status of action</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFood(string id, [FromForm] UpdateFoodViewModel model, IFormFile? image)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ClaimsPrincipal user = HttpContext.User;

            try
            {
                var result = await _foodService.UpdateFoodAsync(id, model, user, image, 800, 350);

                if (result.IsSuccess)
                {
                    return Ok(result); // Return the entire ResultModel if needed
                }
                else
                {
                    return BadRequest(result); // Return the entire ResultModel if needed
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
        #endregion

        #region Delete food
        /// <summary>
        /// Delete a food
        /// </summary>
        /// <returns>Status of action</returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteFood([FromBody] DeleteFoodReqModel request)
        {
            var currentUser = HttpContext.User;
            var result = await _foodService.DeleteFood(request, currentUser);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        #endregion
    }
}