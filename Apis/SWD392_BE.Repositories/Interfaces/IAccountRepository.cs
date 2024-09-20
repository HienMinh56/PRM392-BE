using SWD392_BE.Repositories.Entities;

namespace SWD392_BE.Repositories.Interfaces
{
    public interface IAccountRepository : IGenericRepository<User>
    {
        public Task<User> GetUserByUserName(string userName);

        public Task<User> CheckLogin(string userName, string password);
        public Task<string> GenerateNewUserId();
    }
}
