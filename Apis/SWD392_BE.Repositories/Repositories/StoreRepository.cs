using Microsoft.EntityFrameworkCore;
using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.Helper;
using SWD392_BE.Repositories.Interfaces;
using SWD392_BE.Repositories.ViewModels.StoreModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.Repositories
{
    public class StoreRepository : GenericRepository<Store>, IStoreRepository
    {
        private readonly CampusFoodSystemContext _context;

        public StoreRepository(CampusFoodSystemContext context) : base(context)
        {
            _context = context;
        }
        public async Task<string> GetLastStoreIdAsync()
        {
            // Example query to get the last StoreId
            var lastStore = await _context.Stores
                                            .OrderByDescending(s => s.StoreId)
                                            .FirstOrDefaultAsync();

            return lastStore?.StoreId;
        }

        public async Task<IEnumerable<GetStoreViewModel>> GetStoresByStatusAreaAndSessionAsync(int? status, string? areaName, string? sessionId)
        {
            var query = _context.Stores.Include(s => s.Area)
                              .Include(s => s.StoreSessions)
                              .AsQueryable();

            if (status != null)
                query = query.Where(s => s.Status == status);

            if (!string.IsNullOrEmpty(areaName))
                query = query.Where(s => s.Area.Name == areaName);

            if (!string.IsNullOrEmpty(sessionId))
                query = query.Where(s => s.StoreSessions.Any(ss => ss.SessionId == sessionId));

            // Bỏ qua các điều kiện lọc nếu các giá trị đầu vào là rỗng hoặc null
            if (status == null && string.IsNullOrEmpty(areaName) && string.IsNullOrEmpty(sessionId))
                return await query.Select(s => new GetStoreViewModel
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
                }).ToListAsync();
            else
                return await query.Select(s => new GetStoreViewModel
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
                }).ToListAsync();
        }

        public async Task<List<Store>> FetchStoresAsync()
        {
            return await _context.Stores
                .Include(s => s.Area)
                .Include(s => s.StoreSessions)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Store>> FilterStoresAsync(string? areaId, int? status)
        {
            IQueryable<Store> query = _context.Stores;

            if (!string.IsNullOrEmpty(areaId))
            {
                query = query.Where(s => s.AreaId == areaId);
            }

            if (status.HasValue)
            {
                query = query.Where(s => s.Status == status.Value);
            }

            return await query.ToListAsync();
        }

        public Store GetStoreWithFoods(string storeId)
        {
            return _context.Stores
                           .Include(s => s.Foods)
                           .FirstOrDefault(s => s.StoreId == storeId);
        }

        public async Task<IEnumerable<Store>> SearchStoreByNameOrPhone(string keyword)
        {
            keyword = StringExtensions.RemoveDiacritics(keyword.ToLower().Trim());

            var stores = await _context.Stores
                .AsNoTracking()
                .ToListAsync();

            return stores.Where(s => StringExtensions.RemoveDiacritics
            (s.Name.ToLower()).Contains(keyword)
            || s.Phone.Contains(keyword.Trim()));
        }

        public async Task<List<Store>> GetAllStoresWithSessionsAsync()
        {
            return await _context.Stores
                .Include(s => s.StoreSessions)
                    .ThenInclude(ss => ss.Session)
                .ToListAsync();
        }
    }
}
