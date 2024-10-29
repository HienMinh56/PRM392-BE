using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.ViewModels.AreaModel;
using SWD392_BE.Repositories.ViewModels.StoreModel;
using SWD392_BE.Services.Interfaces;
using SWD392_BE.Services.Services;

namespace SWD392_BE.API.Controllers
{
    [Route("api/v1/area")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        private readonly IAreaService _area;

        public AreaController(IAreaService area)
        {
            _area = area;
        }

        #region Get Areas
        /// <summary>
        /// Get list of areas
        /// </summary>
        /// <returns>A list of areas</returns>
        [HttpGet]
        public async Task<IActionResult> getAreas()
        {
            var result = await _area.getAreas();
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        #endregion

        [HttpGet("{areaId}")]
        public async Task<IActionResult> GetAreaById(string areaId)
        {
            var result = await _area.GetAreaById(areaId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        #region Add Area
        /// <summary>
        /// Add a new area 
        /// </summary>
        /// <returns>Status of action</returns>
        [HttpPost]
        public async Task<IActionResult> AddArea(CreateAreaRequestModel area)
        {
            var currentUser = HttpContext.User;
            var result = await _area.addArea(area, currentUser);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        #endregion

        #region Update area
        /// <summary>
        /// Update a area
        /// </summary>
        /// <returns>Status of action</returns>
        [HttpPut("{areaId}")]
        public async Task<IActionResult> UpdateArea(string areaId, UpdateAreaRequestModel area)
        {
            var currentUser = HttpContext.User;
            var result = await _area.updateAreaAsync(areaId, area, currentUser);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        #endregion

        #region Delete area
        /// <summary>
        /// Delete a area
        /// </summary>
        /// <returns>Status of action</returns>
        [HttpDelete("{areaId}")]
        public async Task<IActionResult> DeleteArea(string areaId)
        {
            var currentUser = HttpContext.User;
            var result = await _area.deleteArea(areaId, currentUser);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        #endregion
    }
}
