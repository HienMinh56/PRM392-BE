using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.Interfaces;
using SWD392_BE.Repositories.Repositories;
using SWD392_BE.Repositories.ViewModels.CampusModel;
using SWD392_BE.Repositories.ViewModels.ResultModel;
using SWD392_BE.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

        public async Task<string> GenerateNewCampusIdAsync()
        {
            var lastCampusId = await _campusRepository.GetLastCampusIdAsync();
            int newIdNumber = 1;

            if (!string.IsNullOrEmpty(lastCampusId))
            {
                // Extract numeric part and increment it
                int.TryParse(lastCampusId.Substring(5), out newIdNumber);
                newIdNumber++;
            }

            // Format the new ID with leading zeros
            string newCampusId = $"CAMP{newIdNumber:D3}";
            return newCampusId;
        }

        public async Task<ResultModel> addCampus(CreateCampusRequestModel campus, ClaimsPrincipal userCreate)
        {
            var result = new ResultModel();
            try
            {
                string newCampusId = await GenerateNewCampusIdAsync();
                if (campus == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Campus request model is null.";
                    return result;
                }

                var existingCampus = _campusRepository.Get(s => s.CampusId == campus.CampusId && s.Name == campus.Name);
                if (existingCampus != null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "A Campus already exists or it's Name is already taken.";
                    return result;
                }

                var newCampus = _mapper.Map<Campus>(campus);
                newCampus.CampusId = newCampusId;
                newCampus.AreaId = campus.AreaId;
                newCampus.Status = 1;
                newCampus.Name = campus.Name;
                newCampus.CreatedBy = userCreate.FindFirst("UserName")?.Value;
                newCampus.CreatedDate = DateTime.Now;

                _campusRepository.Add(newCampus);
                _campusRepository.SaveChanges();
                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Campus added successfully.";
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

        public async Task<ResultModel> deleteCampus(string campusId, ClaimsPrincipal userDelete)
        {
            var result = new ResultModel();
            try
            {
                var existingCampus = _campusRepository.Get(s => s.CampusId == campusId);
                if (existingCampus == null)
                {
                    result.Message = "Area not found or deleted";
                    result.Code = 404;
                    result.IsSuccess = false;
                    result.Data = null;
                    return result;
                }
                existingCampus.DeletedBy = userDelete.FindFirst("UserName")?.Value;
                existingCampus.DeletedDate = DateTime.UtcNow;
                existingCampus.Status = 2;
                _campusRepository.Update(existingCampus);
                _campusRepository.SaveChanges();

                result.Message = "Delete Campus successfully";
                result.Code = 200;
                result.IsSuccess = true;
                result.Data = existingCampus;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
                result.IsSuccess = false;
            }
            return result;
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

        public async Task<ResultModel> GetCampusById(string campusId)
        {
            var result = new ResultModel();
            try
            {
                var campus = _campusRepository.Get(v => v.CampusId == campusId);
                result.IsSuccess = true;
                result.Message = "Get campus successfully";
                result.Data = campus;
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

        public async Task<ResultModel> updateCampusAsync(string campusId, UpdateCampusRequestModel campus, ClaimsPrincipal userUpdate)
        {
            ResultModel result = new ResultModel();
            try
            {
                if (campus == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Area request model is null.";
                    return result;
                }

                var existingCampus = _campusRepository.Get(s => s.CampusId == campusId);
                if (existingCampus == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "Campus not found.";
                    return result;
                }

                var nameCampus = _campusRepository.Get(s => s.Name == campus.Name);
                if (nameCampus != null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "A Campus already exists. Please use another name.";
                    return result;
                }

                // Map the ViewModel to the existing area entity
                _mapper.Map(campus, existingCampus);

                // Update the additional fields
                existingCampus.ModifiedBy = userUpdate.FindFirst("UserName")?.Value;
                existingCampus.ModifiedDate = DateTime.UtcNow;

                _campusRepository.Update(existingCampus);
                await _campusRepository.SaveChangesAsync();

                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Campus updated successfully.";
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
    }
}
