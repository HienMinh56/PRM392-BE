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
    public interface IAccountService
    {
        public Task<ResultModel> AddNewUser(RegisterReqModel model, ClaimsPrincipal user);
        public Task<ResultModel> MobileRegister(CreateMobileViewModel model);
        public Task<ResultModel> SendPasswordResetEmail(string emailTo);


    }
}
