using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.ViewModels.ResultModel;
using SWD392_BE.Repositories.ViewModels.StoreModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Services.Interfaces
{
    public interface IStoreService
    {
        Task<ResultModel> GetStoresByStatusAreaAndSessionAsync(string? Name, string? StoreId, int? status, string? areaName, string? sessionId);
        public Task<ResultModel> AddStore(StoreViewModel storeReqModel, ClaimsPrincipal userCreate);
        public Task<ResultModel> UpdateStoreAsync(string storeId, UpdateStoreViewModel model, ClaimsPrincipal userUpdate);
        public Task<ResultModel> DeleteStore(DeleteStoreReqModel request, ClaimsPrincipal userDelete);
        Task<ResultModel> SearchStoreByNameOrPhone(string keyword);
        Task UpdateStoreStatusAsync();
    }
}
