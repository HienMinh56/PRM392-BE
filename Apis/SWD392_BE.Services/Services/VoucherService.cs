using Microsoft.AspNetCore.Http;
using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.Interfaces;
using SWD392_BE.Repositories.ViewModels.ResultModel;
using SWD392_BE.Repositories.ViewModels.VoucherModel;
using SWD392_BE.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Services.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly IVoucherRepository _voucher;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VoucherService(IVoucherRepository voucher, IHttpContextAccessor httpContextAccessor)
        {
            _voucher = voucher;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResultModel> CreateVoucher(CreateVoucherModel voucherModel, ClaimsPrincipal userCreate)
        {
            var result = new ResultModel();
            try
            {
                var existedVoucher = _voucher.Get(v => v.VoucherCode == voucherModel.VoucherCode);
                if (voucherModel == null)
                {
                    result.IsSuccess = true;
                    result.Message = "Voucher is null";
                    result.Data = null;
                    result.Code = 200;
                    return result;

                }
                if (existedVoucher!=null)
                {
                    result.IsSuccess = false;
                    result.Message = "Voucher code is existed";
                    result.Data = null;
                    result.Code = 400;
                    return result;

                }
                if (voucherModel.ValidityStartDate > voucherModel.ValidityEndDate)
                {
                    result.IsSuccess = false;
                    result.Message = "Invalid date";
                    result.Data = null;
                    result.Code = 400;
                    return result;

                }
                if (voucherModel.ValidityEndDate < DateTime.Now && voucherModel.ValidityEndDate < voucherModel.ValidityStartDate)
                {
                    result.IsSuccess = false;
                    result.Message = "Invalid date";
                    result.Data = null;
                    result.Code = 400;
                    return result;

                }
                var voucher = new Voucher
                {
                    VoucherId = $"VOU{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                    VoucherCode = voucherModel.VoucherCode,
                    DiscountAmount = voucherModel.DiscountAmount,
                    MinOrderAmount = voucherModel.MinOrderAmount,
                    Status = voucherModel.Status,
                    ValidityStartDate = voucherModel.ValidityStartDate,
                    ValidityEndDate = voucherModel.ValidityEndDate,
                    CreatedBy = userCreate.Identity.Name,
                    CreatedDate = DateTime.Now
                };
                result.IsSuccess = true;
                result.Message = "Create voucher successfully";
                result.Data = voucher;
                result.Code = 200;
                _voucher.Add(voucher);
                _voucher.SaveChanges();
                return result;

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.Data = null;
                result.Code = 500;
                return result;

            }

        }

        public async Task<ResultModel> DeleteVoucher(string voucherId, ClaimsPrincipal userCreate)
        {
            var result = new ResultModel();
            try
            {
                var voucher = _voucher.Get(v => v.VoucherId == voucherId);
                if (voucher == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Voucher not found";
                    result.Data = null;
                    result.Code = 400;
                    return result;

                }
                voucher.Status = 0;
                voucher.ModifiedBy = userCreate.Identity.Name;
                voucher.ModifiedDate = DateTime.Now;

                result.IsSuccess = true;
                result.Message = "Delete voucher successfully";
                result.Data = voucher;
                result.Code = 200;
                _voucher.Update(voucher);
                _voucher.SaveChanges();
                return result;

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.Data = null;
                result.Code = 500;
                return result;

            }
        }

        public async Task<ResultModel> GetVoucherById(string id)
        {
            var result = new ResultModel();
            try
            {
                var voucher = _voucher.Get(v => v.VoucherId == id);
                result.IsSuccess = true;
                result.Message = "Get voucher successfully";
                result.Data = voucher;
                result.Code = 200;
                return result;

            }
            catch (Exception ex)
            {
               result.IsSuccess = false;
                result.Message = ex.Message;
                result.Data = null;
                result.Code = 500;
                return result;

            }
        }

        public async Task<ResultModel> GetVouchers()
        {
            var result = new ResultModel();
            try
            {
                var vouchers = _voucher.Get();
                result.IsSuccess = true;
                result.Message = "Get vouchers successfully";
                result.Data = vouchers;
                result.Code = 200;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.Data = null;
                result.Code = 500;
                return result;
            }
        }

        public async Task<ResultModel> UpdateVoucher(string voucherId, CreateVoucherModel voucherModel, ClaimsPrincipal userCreate)
        {
            var result = new ResultModel();
            try
            {
                var voucher = _voucher.Get(v => v.VoucherId == voucherId);
                if (voucher == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Voucher not found";
                    result.Data = null;
                    result.Code = 400;
                    return result;
                }
                if (voucherModel.ValidityStartDate > voucherModel.ValidityEndDate)
                {
                    result.IsSuccess = false;
                    result.Message = "Invalid date";
                    result.Data = null;
                    result.Code = 400;
                    return result;
                }
                if (voucherModel.ValidityEndDate < DateTime.Now && voucherModel.ValidityEndDate < voucherModel.ValidityStartDate)
                {
                    result.IsSuccess = false;
                    result.Message = "Invalid date";
                    result.Data = null;
                    result.Code = 400;
                    return result;
                }
                if(voucher.VoucherCode != voucherModel.VoucherCode)
                {
                    var existedVoucher = _voucher.Get(v => v.VoucherCode == voucherModel.VoucherCode);
                    if (existedVoucher != null)
                    {
                        result.IsSuccess = false;
                        result.Message = "Voucher code is existed";
                        result.Data = null;
                        result.Code = 400;
                        return result;
                    }
                }
                voucher.VoucherCode = voucherModel.VoucherCode;
                voucher.DiscountAmount = voucherModel.DiscountAmount;
                voucher.MinOrderAmount = voucherModel.MinOrderAmount;
                voucher.Status = voucherModel.Status;
                voucher.ValidityStartDate = voucherModel.ValidityStartDate;
                voucher.ValidityEndDate = voucherModel.ValidityEndDate;
                voucher.ModifiedBy = userCreate.Identity.Name;
                voucher.ModifiedDate = DateTime.Now;

                result.IsSuccess = true;
                result.Message = "Update voucher successfully";
                result.Data = voucher;
                result.Code = 200;
                _voucher.Update(voucher);
                _voucher.SaveChanges();
                return result;

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.Data = null;
                result.Code = 500;
                return result;

            }
        }
    }
}
