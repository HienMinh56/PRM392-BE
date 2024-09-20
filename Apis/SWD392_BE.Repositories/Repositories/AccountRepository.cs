using Microsoft.EntityFrameworkCore;
using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.Repositories
{
    public class AccountRepository : GenericRepository<User>, IAccountRepository
    {
        private readonly CampusFoodSystemContext _dbContext;

        public AccountRepository(CampusFoodSystemContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> GetUserByUserName(string userName)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserName.Equals(userName));
        }

        public async Task<User> CheckLogin(string userName, string password)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserName.Equals(userName) && x.Password.Equals(password));
        }

        public async Task<string> GenerateNewUserId()
        {
            const int maxRetryCount = 5;
            int retryCount = 0;

            while (retryCount < maxRetryCount)
            {
                var lastUser = await _dbContext.Users
                                               .Where(u => u.UserId.StartsWith("USER"))
                                               .OrderByDescending(u => u.UserId)
                                               .FirstOrDefaultAsync();

                if (lastUser == null || string.IsNullOrEmpty(lastUser.UserId))
                {
                    return "USER001";
                }

                int newId;
                bool success = int.TryParse(lastUser.UserId.Substring(4), out newId);

                if (!success)
                {
                    throw new InvalidOperationException("Failed to parse UserId.");
                }

                newId += 1;
                string newUserId = $"USER{newId:D3}";

                // Check if the generated userId already exists
                var existingUser = await _dbContext.Users
                                                   .Where(u => u.UserId == newUserId)
                                                   .FirstOrDefaultAsync();

                if (existingUser == null)
                {
                    return newUserId;
                }

                // Increment retry count and try again
                retryCount++;
            }

            throw new InvalidOperationException("Failed to generate a unique UserId after multiple attempts.");
        }


    }
}
