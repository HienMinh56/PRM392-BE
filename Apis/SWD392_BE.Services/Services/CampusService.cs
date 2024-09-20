using AutoMapper;
using SWD392_BE.Repositories.Interfaces;
using SWD392_BE.Repositories.Repositories;
using SWD392_BE.Repositories.ViewModels.ResultModel;
using SWD392_BE.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Services.Services
{
    public class CampusService : ICampusService
    {
        private readonly ICampusRepository _campusRepository;
        private readonly IMapper _mapper;

        public CampusService(ICampusRepository campusRepository, IMapper mapper)
        {
            _campusRepository = campusRepository;
            _mapper = mapper;
        }
        public async Task<ResultModel> getListCampus()
        {
            ResultModel result = new ResultModel();
            try
            {
                var campus = _campusRepository.Get();
                if (campus == null || !campus.Any())
                {
                    result.IsSuccess = true;
                    result.Code = 201;
                    result.Message = "No Campus here";
                }
                else
                {
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Message = "Campus retrieved successfully";
                    result.Data = campus;
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = 500;
                result.Message = $"An error occurred: {ex.Message}";
            }

            return result;
        }
    }
}
