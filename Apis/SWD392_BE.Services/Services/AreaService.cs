using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.Interfaces;
using SWD392_BE.Repositories.Repositories;
using SWD392_BE.Repositories.ViewModels.AreaModel;
using SWD392_BE.Repositories.ViewModels.ResultModel;
using SWD392_BE.Repositories.ViewModels.StoreModel;
using SWD392_BE.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Services.Services
{
    public class AreaService : IAreaService
    {
        private readonly IAreaRepository _areaRepository;
        private readonly IMapper _mapper;

        public AreaService(IAreaRepository areaRepository, IMapper mapper)
        {
            _areaRepository = areaRepository;
            _mapper = mapper;
        }

        public async Task<string> GenerateNewAreaIdAsync()
        {
            var lastAreaId = await _areaRepository.GetLastAreaIdAsync();
            int newIdNumber = 1;

            if (!string.IsNullOrEmpty(lastAreaId))
            {
                // Extract numeric part and increment it
                int.TryParse(lastAreaId.Substring(5), out newIdNumber);
                newIdNumber++;
            }

            // Format the new ID with leading zeros
            string newAreaId = $"AREA{newIdNumber:D3}";
            return newAreaId;
        }

        public async Task<ResultModel> getAreas()
        {
            var result = new ResultModel();
            try
            {
                var areas = _areaRepository.Get();
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

        public async Task<ResultModel> GetAreaById(string areaId)
        {
            var result = new ResultModel();
            try
            {
                var area = _areaRepository.Get(v => v.AreaId == areaId);
                result.IsSuccess = true;
                result.Message = "Get area successfully";
                result.Data = area;
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

        public async Task<ResultModel> addArea(CreateAreaRequestModel area, ClaimsPrincipal userCreate)
        {
            var result = new ResultModel();
            try
            {
                string newAreaId = await GenerateNewAreaIdAsync();
                if (area == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Area request model is null.";
                    return result;
                }

                var existingArea = _areaRepository.Get(s => s.AreaId == area.AreaId && s.Name == area.Name);
                if (existingArea != null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "A Area already exists at the provided address.";
                    return result;
                }

                var newArea = _mapper.Map<Area>(area);
                newArea.AreaId = newAreaId;
                newArea.Status = 1;
                newArea.Name = area.Name;
                newArea.CreatedBy = userCreate.FindFirst("UserName")?.Value;
                newArea.CreatedDate = DateTime.Now;

                _areaRepository.Add(newArea);
                _areaRepository.SaveChanges();
                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Area added successfully.";
                return result;

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = ex.Message;
                return result; ;
            }
        }

        public async Task<ResultModel> updateAreaAsync(string areaId, UpdateAreaRequestModel area, ClaimsPrincipal userUpdate)
        {
            ResultModel result = new ResultModel();
            try
            {
                if (area == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Area request model is null.";
                    return result;
                }

                var existingArea = _areaRepository.Get(s => s.AreaId == areaId);
                if (existingArea == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "Area not found.";
                    return result;
                }

                var nameArea = _areaRepository.Get(s => s.Name == area.Name);
                if (nameArea != null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "A area already exists. Please use another name.";
                    return result;
                }

                // Map the ViewModel to the existing area entity
                _mapper.Map(area, existingArea);

                // Update the additional fields
                existingArea.ModifiedBy = userUpdate.FindFirst("UserName")?.Value;
                existingArea.ModifiedDate = DateTime.UtcNow;

                _areaRepository.Update(existingArea);
                await _areaRepository.SaveChangesAsync();

                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Area updated successfully.";
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = 500;
                result.Message = ex.Message;
                return result;
            }
        }

        public async Task<ResultModel> deleteArea(string areaId, ClaimsPrincipal userDelete)
        {
            var result = new ResultModel();
            try
            {
                var existingArea = _areaRepository.Get(s => s.AreaId == areaId);
                if (existingArea == null)
                {
                    result.Message = "Area not found or deleted";
                    result.Code = 404;
                    result.IsSuccess = false;
                    result.Data = null;
                    return result;
                }
                existingArea.DeletedBy = userDelete.FindFirst("UserName")?.Value;
                existingArea.DeletedDate = DateTime.UtcNow;
                existingArea.Status = 2;
                _areaRepository.Update(existingArea);
                _areaRepository.SaveChanges();

                result.Message = "Delete Area successfully";
                result.Code = 200;
                result.IsSuccess = true;
                result.Data = existingArea;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
                result.IsSuccess = false;
            }
            return result;
        }
    }
}
