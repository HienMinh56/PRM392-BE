using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWD392_BE.Repositories.ViewModels.VoucherModel;
using SWD392_BE.Services.Interfaces;
using System.Security.Claims;

namespace SWD392_BE.API.Controllers
{
    [Route("api/v1/voucher")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;

        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }


        [HttpGet]
        public async Task<IActionResult> GetVouchers()
        {
            var result = await _voucherService.GetVouchers();
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVoucherById(string id)
        {
            var result = await _voucherService.GetVoucherById(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateVoucher([FromBody] CreateVoucherModel voucherModel)
        {
            ClaimsPrincipal user = HttpContext.User;
            var result = await _voucherService.CreateVoucher(voucherModel, user);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{voucherId}")]
        public async Task<IActionResult> UpdateVoucher(string voucherId, [FromBody] CreateVoucherModel voucherModel)
        {
            ClaimsPrincipal user = HttpContext.User;
            var result = await _voucherService.UpdateVoucher(voucherId, voucherModel, user);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{voucherId}")]
        public async Task<IActionResult> DeleteVoucher(string voucherId)
        {
            ClaimsPrincipal user = HttpContext.User;
            var result = await _voucherService.DeleteVoucher(voucherId, user);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

    }
}
