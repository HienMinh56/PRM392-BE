using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.Repositories
{
    public class RefreshTokenRepository : GenericRepository<Token>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(CampusFoodSystemContext context) : base(context)
        {

        }
    }
}
