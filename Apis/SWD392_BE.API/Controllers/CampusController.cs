using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.ViewModels.CampusModel;
using SWD392_BE.Services.Interfaces;

namespace SWD392_BE.API.Controllers
{
    [Route("api/v1/campus")]
    [ApiController]
    public class CampusController : ControllerBase
    {
        private readonly ICampusService _campusService;

        public CampusController(ICampusService campusService)
        {
            _campusService = campusService;
        }

        #region Get List Campus
        /// <summary>
        /// Get list of campuses
        /// </summary>
        /// <returns>A list of campuses</returns>
        [HttpGet]
        public async Task<IActionResult> GetListCampus()
        {
            var result = await _campusService.getListCampus();
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        #endregion

        [HttpGet("{campusId}")]
        public async Task<IActionResult> GetCampusById(string campusId)
        {
            var result = await _campusService.GetCampusById(campusId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        #region Add campus
        /// <summary>
        /// Add a new campus 
        /// </summary>
        /// <returns>Status of action</returns>
        [HttpPost]
        public async Task<IActionResult> AddCampus(CreateCampusRequestModel campus)
        {
            var currentUser = HttpContext.User;
            var result = await _campusService.addCampus(campus, currentUser);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        #endregion

        #region Update campus
        /// <summary>
        /// Update a campus
        /// </summary>
        /// <returns>Status of action</returns>
        [HttpPut("{campusId}")]
        public async Task<IActionResult> Updatecampus(string campusId, UpdateCampusRequestModel campus)
        {
            var currentUser = HttpContext.User;
            var result = await _campusService.updateCampusAsync(campusId, campus, currentUser);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        #endregion

        #region Delete campus
        /// <summary>
        /// Delete a campus
        /// </summary>
        /// <returns>Status of action</returns>
        [HttpDelete("{campusId}")]
        public async Task<IActionResult> Deletecampus(string campusId)
        {
            var currentUser = HttpContext.User;
            var result = await _campusService.deleteCampus(campusId, currentUser);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        #endregion
    }
}