using AutoMapper;
using DevLog.Areas.Panel.Mappings.Resolvers;
using DevLog.Areas.Panel.Models.ViewModels;
using DevLog.Core.Domain;

namespace DevLog.Areas.Panel.Mappings
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<User, UserIndexViewModel>()
                .ForMember(dest => dest.UserFullName,
                    opt =>
                        opt.MapFrom<UserIndexViewModelUserFullNameResolver>());

            CreateMap<UserCreateFormViewModel, User>();
            CreateMap<User, UserFormViewModel>();
            CreateMap<UserFormViewModel, User>();
        }
    }
}
