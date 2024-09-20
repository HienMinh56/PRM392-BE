using SWD392_BE.Repositories.Interfaces;
using SWD392_BE.Repositories.ViewModels.ResultModel;
using SWD392_BE.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Services.Services
{
    public class AreaService : IAreaService
    {
        private readonly IAreaRepository _area;

        public AreaService(IAreaRepository area)
        {
            _area = area;
        }

        public async Task<ResultModel> getAreas()
        {
            var result = new ResultModel();
            try
            {
                var areas = _area.Get();
                if (areas == null)
                {
                    result.IsSuccess = true;
                    result.Code = 201;
                    result.Message = "No Area Here";
                    return result;
                }

                result.IsSuccess = true;
                result.Code = 200;
                result.Data = areas;
                result.Message = "Get Area Success";
                return result;
            }
            catch(Exception ex) 
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = ex.Message;
                return result; ;
            }
        }
    }
}
