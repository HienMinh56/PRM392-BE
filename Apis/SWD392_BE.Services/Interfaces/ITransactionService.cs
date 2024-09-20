using SWD392_BE.Repositories.ViewModels.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<ResultModel> GetTransactionList(string? userId = null, DateTime? createdDate = null);
    }
}
