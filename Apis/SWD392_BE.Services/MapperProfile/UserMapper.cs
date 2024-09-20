using AutoMapper;
using SWD392_BE.Repositories.Entities;
using SWD392_BE.Repositories.ViewModels.FoodModel;
using SWD392_BE.Repositories.ViewModels.OrderModel;
using SWD392_BE.Repositories.ViewModels.StoreModel;
using SWD392_BE.Repositories.ViewModels.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Services.MapperProfile
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<User, ListUserViewModel>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Campus, opt => opt.MapFrom(src => src.Campus.Name))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
            .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            //Regis
            CreateMap<RegisterReqModel, User>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore()) // Ignore UserId since it will be generated
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore()) // Ignore CreatedDate since it will be set separately
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()) // Ignore CreatedBy since it will be set separately
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore()) // Ignore ModifiedDate
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore()) // Ignore ModifiedBy
            .ForMember(dest => dest.DeletedDate, opt => opt.Ignore()) // Ignore DeletedDate
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore()) // Ignore DeletedBy
            .ForMember(dest => dest.Balance, opt => opt.Ignore()) // Ignore Balance
            .ForMember(dest => dest.Campus, opt => opt.Ignore()) // Ignore Campus since it will be handled differently
            .ForMember(dest => dest.Orders, opt => opt.Ignore()) // Ignore Orders
            .ForMember(dest => dest.Transactions, opt => opt.Ignore());

            CreateMap<CreateMobileViewModel, User>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore()) // Ignore UserId since it will be generated
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore()) // Ignore CreatedDate since it will be set separately
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()) // Ignore CreatedBy since it will be set separately
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore()) // Ignore ModifiedDate
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore()) // Ignore ModifiedBy
            .ForMember(dest => dest.DeletedDate, opt => opt.Ignore()) // Ignore DeletedDate
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore()) // Ignore DeletedBy
            .ForMember(dest => dest.Balance, opt => opt.Ignore()) // Ignore Balance
            .ForMember(dest => dest.Campus, opt => opt.Ignore()) // Ignore Campus since it will be handled differently
            .ForMember(dest => dest.Orders, opt => opt.Ignore()) // Ignore Orders
            .ForMember(dest => dest.Transactions, opt => opt.Ignore());

            CreateMap<User, DeleteUserReqModel>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

            
            CreateMap<UpdateUserViewModel, User>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.CampusId, opt => opt.MapFrom(src => src.CampusId))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
            .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance));

            CreateMap<EditUserViewModel, User>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.CampusId, opt => opt.MapFrom(src => src.CampusId))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone));

            //Store
            CreateMap<StoreViewModel, Store>()
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.OpenTime, opt => opt.MapFrom(src => TimeSpan.Parse(src.OpenTime)))
            .ForMember(dest => dest.CloseTime, opt => opt.MapFrom(src => TimeSpan.Parse(src.CloseTime)))
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedDate, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());// Bỏ qua các navigation properties

            CreateMap<UpdateStoreViewModel, Store>()
            .ForMember(dest => dest.OpenTime, opt => opt.MapFrom(src => TimeSpan.Parse(src.OpenTime)))
            .ForMember(dest => dest.CloseTime, opt => opt.MapFrom(src => TimeSpan.Parse(src.CloseTime)));

            CreateMap<Store, DeleteStoreReqModel>()
            .ForMember(dest => dest.StoreId, opt => opt.MapFrom(src => src.StoreId));

            //Food
            CreateMap<FoodViewModel, Food>()
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Cate, opt => opt.MapFrom(src => src.Cate))
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedDate, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());// Bỏ qua các navigation properties

            CreateMap<UpdateFoodViewModel, Food>()
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Cate, opt => opt.MapFrom(src => src.Cate));

            CreateMap<Food, DeleteFoodReqModel>()
            .ForMember(dest => dest.FoodId, opt => opt.MapFrom(src => src.FoodId));

            CreateMap<Order, OrderListViewModel>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.SessionId))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store.Name))
            .ForMember(dest => dest.TransationId, opt => opt.MapFrom(src => src.TransactionId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.CreatedTime, opt => opt.Ignore());
        }
    }
}
