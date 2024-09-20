using Microsoft.EntityFrameworkCore;
using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.Interfaces;
using SWD392_BE.Repositories.ViewModels.FoodModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.Repositories
{
    public class FoodRepository : GenericRepository<Food>, IFoodRepository
    {
        private readonly CampusFoodSystemContext _context;

        public FoodRepository(CampusFoodSystemContext context) : base(context)
        {
            _context = context;
        }
        public async Task<string> GetLastFoodIdAsync()
        {
            // Example query to get the last StoreId
            var lastFood = await _context.Foods
                                            .OrderByDescending(s => s.FoodId)
                                            .FirstOrDefaultAsync();

            return lastFood?.FoodId;
        }

        public async Task<string> GetStoreNameAsync(string storeId)
        {
            var store = await _context.Stores.FirstOrDefaultAsync(s => s.StoreId == storeId);
            return store?.Name ?? "Unknown Store";
        }

        public async Task<int> GetTotalFoodsAsync(string storeId, int? cate)
        {
            var query = _context.Foods.Where(f => f.StoreId == storeId);

            if (cate.HasValue)
            {
                query = query.Where(f => f.Cate == cate.Value);
            }

            return await query.CountAsync();
        }

        public async Task<int> GetTotalOrdersAsync(string storeId)
        {
            return await _context.Orders
                                 .Where(o => o.StoreId == storeId && o.Status == 1) // Assuming status 1 means active order
                                 .CountAsync();
        }
        public Task<Food> GetAsync(Expression<Func<Food, bool>> predicate)
        {
            return _context.Foods.FirstOrDefaultAsync(predicate);
        }
        public async Task<List<FoodOrderCount>> GetFoodOrderCountsAsync(string storeId)
        {
            return await _context.OrderDetails
                .Where(o => o.Food.StoreId == storeId)
                .GroupBy(o => o.FoodId)
                .Select(g => new FoodOrderCount
                {
                    FoodId = g.Key,
                    OrderCount = g.Count()
                })
                .ToListAsync();
        }
    }
}
