using Microsoft.EntityFrameworkCore;
using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.ViewModels.FoodModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.Interfaces
{
    public interface IFoodRepository : IGenericRepository<Food>
    {
        Task<string> GetLastFoodIdAsync();
        Task<string> GetStoreNameAsync(string storeId);
        Task<int> GetTotalFoodsAsync(string storeId, int? cate);
        Task<int> GetTotalOrdersAsync(string storeId);
        Task<Food> GetAsync(Expression<Func<Food, bool>> predicate);
        Task<List<FoodOrderCount>> GetFoodOrderCountsAsync(string storeId);
    }
}
