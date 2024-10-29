using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.ViewModels.StoreModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.Interfaces
{
    public interface IStoreRepository : IGenericRepository<Store>
    {
        Task<string> GetLastStoreIdAsync();
        Task<List<Store>> FetchStoresAsync();
        public Store GetStoreWithFoods(string storeId);
        Task<IEnumerable<GetStoreViewModel>> GetStoresByStatusAreaAndSessionAsync(int? status, string? areaName, string? sessionId);
        Task<IEnumerable<Store>?> SearchStoreByNameOrPhone(string keyword);
        Task<List<Store>> GetAllStoresWithSessionsAsync();
    }
}
