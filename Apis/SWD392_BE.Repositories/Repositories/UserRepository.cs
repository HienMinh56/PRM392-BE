using Microsoft.AspNetCore.CookiePolicy;
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
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly CampusFoodSystemContext _dbContext;
        public UserRepository(CampusFoodSystemContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<User>> GetUsers()
        {
            return await _dbContext.Users
                .Include(x => x.Campus)
                .ThenInclude(c => c.Area)
                .Include(x => x.Orders)              
                .Select(x => new User
                {
                    UserId = x.UserId,
                    Name = x.Name,
                    UserName = x.UserName,
                    Password = x.Password,
                    Email = x.Email,
                    Campus = new Campus
                    {
                        CampusId = x.Campus.CampusId,
                        Name = x.Campus.Name,
                        Area = new Area
                        {
                            AreaId = x.Campus.Area.AreaId,
                            Name = x.Campus.Area.Name
                        }
                    },
                    Phone = x.Phone,
                    Role = x.Role,
                    Status = x.Status,
                    Balance = x.Balance,
                    CreatedDate = x.CreatedDate,
                    CreatedBy = x.CreatedBy,
                })
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<User> GetUserByUserName(string userName)
        {
            return await _dbContext.Users
                .Where(x => x.UserName == userName)
                .Select(x => new User
                {
                    UserId = x.UserId,
                    Name = x.Name,
                    UserName = x.UserName,
                    Password = x.Password,
                    Email = x.Email,
                    CampusId = x.CampusId,
                    Phone = x.Phone,
                    Role = x.Role,
                    Balance = x.Balance,
                    Status = x.Status,
                    CreatedDate = x.CreatedDate,
                    CreatedBy = x.CreatedBy,
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
        public IQueryable<User> GetAll()
        {
            return _dbContext.Users.AsQueryable(); 
        }
    }
}
