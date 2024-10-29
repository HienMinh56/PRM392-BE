using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.ViewModels.TransactionModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.Interfaces
{
    public interface ITransactionRepository : IGenericRepository<Transaction>
    {
        IEnumerable<Transaction> GetRecentTransactions();
        Transaction GetByTransactionId(string transactionId);
        Transaction GetLatestTransaction();
        Transaction GetById(int id);
        void Update(Transaction transaction);
        void SaveChanges();
        Task SaveChangesAsync();
        Task<Transaction> AddTransaction(Transaction transaction);
        Task<List<TransactionUserViewModel>?> GetTransaction(string? userId = null, DateTime? createdDate = null);
        Task<List<Transaction>> GetAllTransactionsAsync();

    }
}
