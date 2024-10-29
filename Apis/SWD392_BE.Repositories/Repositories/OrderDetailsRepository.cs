using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.Repositories
{
    public class OrderDetailsRepository : GenericRepository<OrderDetail>, IOrderDetailsRepository
    {
        private readonly CampusFoodSystemContext _dbContext;
        public OrderDetailsRepository(CampusFoodSystemContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<OrderDetail> GetQuery() => _dbContext.OrderDetails.AsQueryable();
    }
}
