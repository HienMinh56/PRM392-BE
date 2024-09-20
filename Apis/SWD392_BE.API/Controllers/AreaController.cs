using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SWD392_BE.Services.Interfaces;

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
    }
}
