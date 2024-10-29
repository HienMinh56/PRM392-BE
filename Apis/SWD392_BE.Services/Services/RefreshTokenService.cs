using Microsoft.EntityFrameworkCore;
using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.Interfaces;
using SWD392_BE.Repositories.Repositories;
using SWD392_BE.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Services.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        public void GenerateRefreshToken(Token token)
        {
            try
            {
                var existingToken = _refreshTokenRepository.Get(x => x.UserId == token.UserId);
                if (existingToken != null)
                {
                    existingToken.AccessToken = token.AccessToken;
                    existingToken.RefreshToken = token.RefreshToken;
                    existingToken.ExpiredTime = token.ExpiredTime;
                    existingToken.Status = token.Status;
                    _refreshTokenRepository.Update(existingToken);
                }
                else
                {
                    _refreshTokenRepository.Add(token);
                }
                _refreshTokenRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Token GetRefreshToken(string refreshToken)
        {
            try
            {
                var _refreshToken = _refreshTokenRepository.Get(x => x.RefreshToken == refreshToken);
                return _refreshToken;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Token GetRefreshTokenByUserID(string userID)
        {
            try
            {
                var existingToken = _refreshTokenRepository.Get(x => x.UserId == userID);
                return existingToken;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void RemoveAllRefreshToken()
        {
            try
            {
                var _refreshTokenList = _refreshTokenRepository.Get();
                foreach (var item in _refreshTokenList)
                {
                    _refreshTokenRepository.Remove(item);
                }
                _refreshTokenRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void ResetRefreshToken()
        {
            try
            {
                var _refreshToken = (List<Token>)_refreshTokenRepository.Get();
                foreach (var item in _refreshToken)
                {
                    if (item.Status == 2 || item.ExpiredTime <= DateTime.Now)
                    {
                        _refreshTokenRepository.Remove(item);
                        _refreshTokenRepository.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateRefreshToken(Token _refreshToken)
        {
            try
            {
                _refreshToken.Status = 2;
                _refreshTokenRepository.Update(_refreshToken);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

