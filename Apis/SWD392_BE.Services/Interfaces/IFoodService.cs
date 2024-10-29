using Microsoft.AspNetCore.Http;
using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.ViewModels.FoodModel;
using SWD392_BE.Repositories.ViewModels.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Services.Interfaces
{
    public interface IFoodService
    {
        public Task<ResultModel> GetListFoodsAsync(string? foodId, string? storeId, int? cate);
        public Task<ResultModel> addFood(string storeId, List<FoodViewModel> foodLists, ClaimsPrincipal userCreate, int maxWidth, int maxHeight);
        public Task<ResultModel> UpdateFoodAsync(string id, UpdateFoodViewModel model, ClaimsPrincipal userUpdate, IFormFile? image, int maxWidth, int maxHeight);
        public Task<ResultModel> DeleteFood(DeleteFoodReqModel request, ClaimsPrincipal userDelete);
    }
}
