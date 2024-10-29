using SWD392_BE.Repositories.ViewModels.ResultModel;
using SWD392_BE.Repositories.ViewModels.VoucherModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Services.Interfaces
{
    public interface IVoucherService
    {
        public Task<ResultModel> GetVouchers();
        public Task<ResultModel> GetVoucherById(string id);
        public Task<ResultModel> CreateVoucher(CreateVoucherModel voucherModel, ClaimsPrincipal userCreate);
        public Task<ResultModel> UpdateVoucher(string voucherId, CreateVoucherModel voucherModel, ClaimsPrincipal userCreate);
        public Task<ResultModel> DeleteVoucher(string voucherId, ClaimsPrincipal userCreate);
    }
}
