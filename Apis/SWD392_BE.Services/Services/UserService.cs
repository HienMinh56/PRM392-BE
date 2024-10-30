using AutoMapper;
using Google.Apis.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.Interfaces;
using SWD392_BE.Repositories.Repositories;
using SWD392_BE.Repositories.ViewModels.PageModel;
using SWD392_BE.Repositories.ViewModels.ResultModel;
using SWD392_BE.Repositories.ViewModels.UserModel;
using SWD392_BE.Services.Interfaces;
using SWD392_BE.Services.Sercurity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICampusRepository _campusRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, ICampusRepository campusRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _campusRepository = campusRepository;
            _mapper = mapper;
        }

        public async Task<ResultModel> GetUserList(string? userId, string? name, string? email, string? phone, int? status, string? campusName, string? areaName)
        {
            var result = new ResultModel();
            try
            {
                var users = await _userRepository.GetUsers();

                if (!string.IsNullOrEmpty(userId))
                {
                    users = users.Where(u => u.UserId.ToLowerInvariant() == userId.ToLowerInvariant()).ToList();
                }

                if (!string.IsNullOrEmpty(name))
                {
                    users = users.Where(u => u.Name.ToLowerInvariant() == name.ToLowerInvariant()).ToList();
                }

                if (!string.IsNullOrEmpty(email))
                {
                    users = users.Where(u => u.Email.ToLowerInvariant() == email.ToLowerInvariant()).ToList();
                }

                if (!string.IsNullOrEmpty(phone))
                {
                    users = users.Where(u => u.Phone.ToLowerInvariant() == phone.ToLowerInvariant()).ToList();
                }

                if (status.HasValue)
                {
                    users = users.Where(u => u.Status == status.Value).ToList();
                }

                if (!string.IsNullOrEmpty(campusName))
                {
                    users = users.Where(u => u.Campus.Name.ToLowerInvariant() == campusName.ToLowerInvariant()).ToList();
                }

                if (!string.IsNullOrEmpty(areaName))
                {
                    users = users.Where(u => u.Campus.Area.Name.ToLowerInvariant() == areaName.ToLowerInvariant()).ToList();
                }

                if (!users.Any())
                {
                    result.Message = "Data not found";
                    result.IsSuccess = false;
                    result.Code = 404;
                }
                else
                {
                    users = users.OrderByDescending(u => u.UserId).ToList();

                    var userViewModels = users.Select(u => new ListUserViewModel
                    {
                        UserId = u.UserId,
                        Name = u.Name,
                        UserName = u.UserName,
                        Password = u.Password,
                        Email = u.Email,
                        Campus = u.Campus.Name,
                        Area = u.Campus.Area.Name,
                        Phone = u.Phone,
                        Role = u.Role,
                        Balance = u.Balance,
                        Status = u.Status,
                        CreatedDate = u.CreatedDate
                    }).ToList();

                    result.Data = userViewModels;
                    result.Message = "Success";
                    result.IsSuccess = true;
                    result.Code = 200;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.IsSuccess = false;
                result.Code = 500; // Add the status code for error
            }
            return result;
        }



        public User GetUserById(string id)
        {
            try
            {
                var user = _userRepository.Get(x => x.UserId == id);

                if (user == null)
                {
                    return null;
                }

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public User GetUserByUserName(string userName)
        {
            var user = _userRepository.Get(u => u.UserName == userName);
            if (user == null)
            {
                return null;
            }
            return user;
        }
        public User SearchUser(string keyword)
        {
            var user = _userRepository.Get(u => u.UserName.Contains(keyword.Trim())
                                                || u.Email.Contains(keyword.Trim())
                                                || u.Phone.Contains(keyword.Trim()));
            if (user != null)
            {
                return user;
            }
            return null;
        }
        private int checkNameAndEmail(string name, string email, string userId)
        {
            var users = _userRepository.Get();
            if (users.FirstOrDefault(c => c.Name == name && c.UserId != userId) != null)
            {
                return 0;
            }
            else if (users.FirstOrDefault(d => d.Email == email && d.UserId != userId) != null)
            {
                return -1;
            }
            return 1;
        }
        public async Task<ResultModel> UpdateUser(string userId, UpdateUserViewModel model, ClaimsPrincipal userUpdate)

        {
            var result = new ResultModel();
            try
            {
                var existingUser = _userRepository.Get(x => x.UserId == userId);
                if (existingUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "Can not find user";
                    return result;
                }
                // Check if email already exists
                var existingEmail = _userRepository.Get(x => x.Email == model.Email && x.UserId != userId);
                if (existingEmail != null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Email already exists";
                    return result;
                }
                // Check if Phone already exists
                var existingPhone = _userRepository.Get(x => x.Phone == model.Phone && x.UserId != userId);
                if (existingPhone != null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Phone already exists";
                    return result;
                }
                // Map the ViewModel to the existing userid entity
                _mapper.Map(model, existingUser);

                // Update the additional fields
                existingUser.Name = model.Name;
                existingUser.Email = model.Email;
                if (!string.IsNullOrWhiteSpace(model.CampusId))
                {
                    existingUser.CampusId = model.CampusId;
                }

                existingUser.Phone = model.Phone;
                existingUser.Role = model.Role;
                existingUser.Balance = model.Balance;

                existingUser.ModifiedBy = userUpdate.FindFirst("UserName")?.Value;
                existingUser.ModifiedDate = DateTime.Now;
                _userRepository.Update(existingUser);
                _userRepository.SaveChanges();
                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Update User Success";
                return result;

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = ex.Message;
                return result;
            }
            return result;
        }
        public async Task<ResultModel> EditUser(string userId, EditUserViewModel model, ClaimsPrincipal userUpdate)
        {
            var result = new ResultModel();
            try
            {
                var existingUser = _userRepository.Get(x => x.UserId == userId);
                if (existingUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "Cannot find user";
                    return result;
                }

                // Check if email already exists
                var existingEmail = _userRepository.Get(x => x.Email == model.Email && x.UserId != userId);
                if (existingEmail != null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Email already exists";
                    return result;
                }

                // Check if Phone already exists
                var existingPhone = _userRepository.Get(x => x.Phone == model.Phone && x.UserId != userId);
                if (existingPhone != null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Phone already exists";
                    return result;
                }

                // Check if the old password is correct
                if (!PasswordHasher.VerifyPassword(model.OldPassword, existingUser.Password))
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Old password is incorrect";
                    return result;
                }

                // Check if the new password and confirm password match
                if (model.NewPassword != model.ConfirmPassword)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "New password and confirm password do not match";
                    return result;
                }

                // Map the ViewModel to the existing user entity
                _mapper.Map(model, existingUser);

                // Update the additional fields
                existingUser.Name = model.Name;
                existingUser.Password = PasswordHasher.HashPassword(model.NewPassword); // Update the password
                existingUser.Email = model.Email;
                existingUser.Phone = model.Phone;
                existingUser.ModifiedBy = userUpdate.FindFirst("UserName")?.Value;
                existingUser.ModifiedDate = DateTime.Now;

                _userRepository.Update(existingUser);
                _userRepository.SaveChanges();

                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Edit User Success";
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = ex.Message;
                return result;
            }
        }
        public async Task<ResultModel> DeleteUser(DeleteUserReqModel request, ClaimsPrincipal userDelete)
        {
            var result = new ResultModel();
            try
            {
                var user = GetUserById(request.UserId);
                if (user == null)
                {
                    result.Message = "UserName not found or deleted";
                    result.Code = 404;
                    result.IsSuccess = false;
                    result.Data = null;
                    return result;
                }
                user.DeletedBy = userDelete.FindFirst("UserName")?.Value;
                user.DeletedDate = DateTime.UtcNow;
                user.Status = user.Status = 2;
                _userRepository.Update(user);
                _userRepository.SaveChanges();

                result.Message = "Delete user successfully";
                result.Code = 200;
                result.IsSuccess = true;
                result.Data = user;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
                result.IsSuccess = false;
            }
            return result;
        }
        public async Task<ResultModel> SearchUserByKeyword(string keyword)
        {
            var result = new ResultModel();
            try
            {
                var user = SearchUser(keyword);
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = user;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.Code = 404;
                return result;
            }
        }

        public async Task<ResultModel> UpdateUserBalance(string userId, int amount)
        {
            var result = new ResultModel();
            try
            {
                var existingUser = _userRepository.Get(x => x.UserId == userId);
                if (existingUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "User not found";
                    return result;
                }
                if (amount < 0 && existingUser.Balance + amount < 0)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Not enough balance";
                    return result;
                }
                existingUser.Balance += amount;
                _userRepository.Update(existingUser);
                await _userRepository.SaveChangesAsync();

                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "User balance updated successfully";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = 500;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<int> GetTotalUserCountAsync()
        {
            return await _userRepository.GetAll().CountAsync();
        }
    }
}