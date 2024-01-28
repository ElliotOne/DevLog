using AutoMapper;
using DevLog.Areas.Panel.Mappings.Resolvers;
using DevLog.Areas.Panel.Models.ViewModels;
using DevLog.Core.Domain;

namespace DevLog.Areas.Panel.Mappings
{
    public class PostMapper : Profile
    {
        public PostMapper()
        {
            CreateMap<Post, PostIndexViewModel>()
                .ForMember(dest => dest.UserFullName,
                    opt =>
                        opt.MapFrom<PostViewModelsUserFullNameResolver<PostIndexViewModel>>());

            CreateMap<Post, PostFormViewModel>()
                .ForMember(dest => dest.UserFullName,
                    opt =>
                        opt.MapFrom<PostViewModelsUserFullNameResolver<PostFormViewModel>>());

            CreateMap<PostFormViewModel, Post>();
        }
    }
}
