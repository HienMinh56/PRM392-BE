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
    public class AreaRepository : GenericRepository<Area>, IAreaRepository
    {
        private readonly CampusFoodSystemContext _context;

        public AreaRepository(CampusFoodSystemContext context) : base(context)
        {
            _context = context;
        }

        public async Task<string> GetLastAreaIdAsync()
        {
            // Example query to get the last StoreId
            var lastArea = await _context.Areas
                                            .OrderByDescending(s => s.AreaId)
                                            .FirstOrDefaultAsync();

            return lastArea?.AreaId;
        }
    }
}
