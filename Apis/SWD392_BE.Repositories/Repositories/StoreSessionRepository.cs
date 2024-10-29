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
    public class StoreSessionRepository : GenericRepository<StoreSession>, IStoreSessionRepository
    {
        private readonly CampusFoodSystemContext _context;

        public StoreSessionRepository(CampusFoodSystemContext context) : base(context)
        {
            _context = context;
        }

    }
}
