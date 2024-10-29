using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.Interfaces;
using SWD392_BE.Repositories.Repositories;
using SWD392_BE.Repositories.ViewModels.FoodModel;
using SWD392_BE.Repositories.ViewModels.PageModel;
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
    public class StoreService : IStoreService
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IStoreSessionRepository _storeSession;
        private readonly ISessionRepository _session;
        private readonly IMapper _mapper;

        public StoreService(IStoreRepository storeRepository, IStoreSessionRepository storeSession, ISessionRepository session, IMapper mapper)
        {
            _storeRepository = storeRepository;
            _storeSession = storeSession;
            _session = session;
            _mapper = mapper;
        }

        public async Task<string> GenerateNewStoreIdAsync()
        {
            var lastStoreId = await _storeRepository.GetLastStoreIdAsync();
            int newIdNumber = 1;

            if (!string.IsNullOrEmpty(lastStoreId))
            {
                // Extract numeric part and increment it
                int.TryParse(lastStoreId.Substring(5), out newIdNumber);
                newIdNumber++;
            }

            // Format the new ID with leading zeros
            string newStoreId = $"STORE{newIdNumber:D3}";
            return newStoreId;
        }

        public Store GetStoreById(string id)
        {
            try
            {
                var data = _storeRepository.Get(x => x.StoreId == id);

                if (data == null)
                {
                    // Handle the case where the user is not found, e.g., return null or throw an exception
                    return null;
                }

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResultModel> AddStore(StoreViewModel model, ClaimsPrincipal userCreate)
        {
            ResultModel result = new ResultModel();
            try
            {
                string newStoreId = await GenerateNewStoreIdAsync();

                if (model == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Store request model is null.";
                    return result;
                }

                var existingStore = _storeRepository.Get(s => s.AreaId == model.AreaId && s.Address == model.Address);
                if (existingStore != null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "A store already exists at the provided address.";
                    return result;
                }

                var existingPhone = _storeRepository.Get(s => s.Phone == model.Phone);
                if (existingPhone != null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Phone number has already been used.";
                    return result;
                }

                var openTime = TimeSpan.Parse(model.OpenTime);
                var closeTime = TimeSpan.Parse(model.CloseTime);

                // Find all sessions that the store's operating hours span
                var matchingSessions = await _session.FindAsync(s =>
                    (openTime < s.EndTime && closeTime > s.StartTime));

                if (!matchingSessions.Any())
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "No session found for the provided open and close times.";
                    return result;
                }

                var newStore = _mapper.Map<Store>(model);
                newStore.StoreId = newStoreId;
                newStore.CreatedBy = userCreate.FindFirst("UserName")?.Value;
                newStore.CreatedDate = DateTime.Now;
                newStore.Status = 1;
                newStore.OpenTime = openTime;
                newStore.CloseTime = closeTime;

                _storeRepository.Add(newStore);
                _storeRepository.SaveChanges();

                foreach (var session in matchingSessions)
                {
                    var storeSession = new StoreSession
                    {
                        StoreId = newStoreId,
                        SessionId = session.SessionId
                    };
                    _storeSession.Add(storeSession);
                }

                _storeSession.SaveChanges();

                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Store added successfully.";
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = ex.Message;
                return result;
            }
        }

        public async Task<ResultModel> GetStoresByStatusAreaAndSessionAsync(string? Name, string? StoreId, int? status, string? areaName, string? sessionId)
        {
            ResultModel result = new ResultModel();
            try
            {
                var stores = await _storeRepository.FetchStoresAsync();

                if (!string.IsNullOrEmpty(Name))
                {
                    stores = stores.Where(s => s.Name.ToLower() == Name.ToLower()).ToList();
                }

                if (!string.IsNullOrEmpty(StoreId))
                {
                    stores = stores.Where(s => s.StoreId.ToLower() == StoreId.ToLower()).ToList();
                }

                if (status.HasValue)
                {
                    stores = stores.Where(s => s.Status == status.Value).ToList();
                }

                if (!string.IsNullOrEmpty(areaName))
                {
                    stores = stores.Where(s => s.Area.Name.ToLower() == areaName.ToLower()).ToList();
                }

                if (!string.IsNullOrEmpty(sessionId))
                {
                    stores = stores.Where(s => s.StoreSessions.Any(ss => ss.SessionId.ToLower() == sessionId.ToLower())).ToList();
                }

                if (!stores.Any())
                {
                    result.IsSuccess = true;
                    result.Code = 201;
                    result.Message = "No stores found";
                }
                else
                {
                    stores = stores.OrderByDescending(s => s.StoreId).ToList();

                    var storeViewModels = stores.Select(s => new GetStoreViewModel
                    {
                        StoreId = s.StoreId,
                        AreaId = s.AreaId,
                        Name = s.Name,
                        Address = s.Address,
                        Status = s.Status,
                        Phone = s.Phone,
                        OpenTime = s.OpenTime,
                        CloseTime = s.CloseTime,
                        AreaName = s.Area.Name,
                        Session = s.StoreSessions.Select(ss => ss.SessionId.ToString()).ToList()
                    }).ToList();

                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Message = "Stores retrieved successfully";
                    result.Data = storeViewModels;
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


        public async Task<ResultModel> UpdateStoreAsync(string storeId, UpdateStoreViewModel model, ClaimsPrincipal userUpdate)
        {
            ResultModel result = new ResultModel();
            try
            {
                if (model == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Store request model is null.";
                    return result;
                }

                var existingStore = _storeRepository.Get(s => s.StoreId == storeId);
                if (existingStore == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "Store not found.";
                    return result;
                }

                var phoneStore = _storeRepository.Get(s => s.Phone == model.Phone && s.StoreId != storeId);
                if (phoneStore != null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Phone number has already been used.";
                    return result;
                }

                var addressStore = _storeRepository.Get(s => s.Address == model.Address && s.AreaId == model.AreaId && s.StoreId != storeId);
                if (addressStore != null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "A store already exists at the provided address.";
                    return result;
                }

                var openTime = TimeSpan.Parse(model.OpenTime);
                var closeTime = TimeSpan.Parse(model.CloseTime);

                // Find all sessions that the store's operating hours span
                var matchingSessions = await _session.FindAsync(s =>
                    (openTime < s.EndTime && closeTime > s.StartTime));

                if (!matchingSessions.Any())
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "No session found for the provided open and close times.";
                    return result;
                }

                // Remove existing store sessions
                var existingStoreSessions = _storeSession.Get().Where(ss => ss.StoreId == storeId).ToList();
                foreach (var storeSession in existingStoreSessions)
                {
                    _storeSession.Remove(storeSession);
                }

                // Add new store sessions
                foreach (var session in matchingSessions)
                {
                    var storeSession = new StoreSession
                    {
                        StoreId = storeId,
                        SessionId = session.SessionId
                    };
                    _storeSession.Add(storeSession);
                }

                // Map the ViewModel to the existing store entity
                _mapper.Map(model, existingStore);

                // Update the additional fields
                existingStore.ModifiedBy = userUpdate.FindFirst("UserName")?.Value;
                existingStore.ModifiedDate = DateTime.UtcNow;
                existingStore.OpenTime = openTime;
                existingStore.CloseTime = closeTime;

                _storeRepository.Update(existingStore);
                await _storeRepository.SaveChangesAsync();
                await _storeSession.SaveChangesAsync();

                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Store updated successfully.";
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

        public async Task<ResultModel> DeleteStore(DeleteStoreReqModel request, ClaimsPrincipal userDelete)
        {
            var result = new ResultModel();
            try
            {
                var store = GetStoreById(request.StoreId);
                if (store == null)
                {
                    result.Message = "Store not found or deleted";
                    result.Code = 404;
                    result.IsSuccess = false;
                    result.Data = null;
                    return result;
                }
                store.DeletedBy = userDelete.FindFirst("UserName")?.Value;
                store.DeletedDate = DateTime.UtcNow;
                store.Status = store.Status = 2;
                _storeRepository.Update(store);
                _storeRepository.SaveChanges();

                result.Message = "Delete Store successfully";
                result.Code = 200;
                result.IsSuccess = true;
                result.Data = store;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
                result.IsSuccess = false;
            }
            return result;
        }

        public async Task<ResultModel> SearchStoreByNameOrPhone(string keyword)
        {
            var result = new ResultModel();
            try
            {
                var store = await _storeRepository.SearchStoreByNameOrPhone(keyword);
                if (store == null)
                {
                    result.Message = "Store not found";
                    result.Code = 404;
                    result.IsSuccess = false;
                    result.Data = null;
                    return result;
                }
                result.Message = "Store found";
                result.Code = 200;
                result.IsSuccess = true;
                result.Data = store;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.IsSuccess = false;
            }
            return result;
        }

        public async Task UpdateStoreStatusAsync()
        {
            var stores = await _storeRepository.GetAllStoresWithSessionsAsync();

            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone).TimeOfDay;

            foreach (var store in stores)
            {
                bool isOpen = false;

                foreach (var storeSession in store.StoreSessions)
                {
                    var session = storeSession.Session;

                    if (localTime >= store.OpenTime && localTime <= store.CloseTime &&
                        localTime >= session.StartTime && localTime <= session.EndTime)
                    {
                        isOpen = true;
                        break;
                    }
                }

                store.Status = isOpen ? 1 : 2;
            }

            await _storeRepository.SaveChangesAsync();
        }
    }
}
