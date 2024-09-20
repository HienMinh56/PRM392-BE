using Microsoft.AspNetCore.Mvc;
using SWD392_BE.Services.Interfaces;

namespace SWD392_BE.API.Controllers
{
    [Route("api/v1/transaction")]
    [ApiController]
    public class TransactionController : Controller
    {
        private readonly ITransactionService _transactionService;
        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactionList(string? userId = null, DateTime? createdDate = null)
        {
            var result = await _transactionService.GetTransactionList(userId, createdDate);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
