using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.ViewModels.AreaModel;
using SWD392_BE.Repositories.ViewModels.CampusModel;
using SWD392_BE.Repositories.ViewModels.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Services.Interfaces
{
    public interface ICampusService
    {
        public Task<ResultModel> getListCampus();
        public Task<ResultModel> GetCampusById(string campusId);
        Task<ResultModel> addCampus(CreateCampusRequestModel campus, ClaimsPrincipal userCreate);
        Task<ResultModel> updateCampusAsync(string campusId, UpdateCampusRequestModel campus, ClaimsPrincipal userUpdate);
        Task<ResultModel> deleteCampus(string campusId, ClaimsPrincipal userDelete);
    }
}
