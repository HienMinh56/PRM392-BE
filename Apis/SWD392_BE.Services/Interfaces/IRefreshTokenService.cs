using SWD392_BE.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Services.Interfaces
{
    public interface IRefreshTokenService
    {
        public void GenerateRefreshToken(Token refreshToken);
        public void ResetRefreshToken();
        public Token GetRefreshToken(string refreshToken);
        public Token GetRefreshTokenByUserID(string userID);
        public void UpdateRefreshToken(Token refreshToken);
        public void RemoveAllRefreshToken();
    }
}
