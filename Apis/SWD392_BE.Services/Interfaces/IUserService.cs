using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.ViewModels.ResultModel;
using SWD392_BE.Repositories.ViewModels.ResultModel;
using SWD392_BE.Repositories.ViewModels.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Services.Interfaces
{
    public interface IUserService
    {
        Task<ResultModel> GetUserList(string? userId, string? name, string? email, string? phone, int? status, string? campusName, string? areaName);
        public Task<ResultModel> DeleteUser(DeleteUserReqModel request, ClaimsPrincipal userDelete);
        public User GetUserById(string id);
        public User GetUserByUserName(string userName);
        public Task<ResultModel> SearchUserByKeyword(string keyword);
        public Task<ResultModel> UpdateUser(string userId ,UpdateUserViewModel model, ClaimsPrincipal userUpdate);
        public Task<ResultModel> EditUser(string userId, EditUserViewModel model, ClaimsPrincipal userUpdate);
        Task<ResultModel> UpdateUserBalance(string userId, int amount);
        Task<int> GetTotalUserCountAsync();
    }
}
