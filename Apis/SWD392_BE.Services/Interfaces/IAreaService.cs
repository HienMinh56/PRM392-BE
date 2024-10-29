using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.ViewModels.AreaModel;
using SWD392_BE.Repositories.ViewModels.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Services.Interfaces
{
    public interface IAreaService
    {
        Task<ResultModel> getAreas();
        public Task<ResultModel> GetAreaById(string areaId);
        Task<ResultModel> addArea(CreateAreaRequestModel area, ClaimsPrincipal userCreate);
        Task<ResultModel> updateAreaAsync(string areaId, UpdateAreaRequestModel area, ClaimsPrincipal userUpdate);
        Task<ResultModel> deleteArea(string areaId, ClaimsPrincipal userDelete);
    }
}
