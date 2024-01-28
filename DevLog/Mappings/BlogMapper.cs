using AutoMapper;
using DevLog.Core.Domain;
using DevLog.Mappings.Resolvers;
using DevLog.Models.ViewModels;

namespace DevLog.Mappings
{
    public class BlogMapper : Profile
    {
        public BlogMapper()
        {
            CreateMap<PostCategory, PostCategoryViewModel>();

            CreateMap<Post, PostViewModel>()
                .ForMember(dest => dest.PostCategoryViewModel,
                    opt =>
                        opt.MapFrom(src => src.PostCategory))
                .ForMember(dest => dest.UserFullName,
                    opt =>
                        opt.MapFrom<PostViewModelUserFullNameResolver>())
                .ForMember(dest => dest.PostCommentViewModels,
                    opt =>
                        opt.MapFrom(src => src.PostComments));

            CreateMap<PostComment, PostCommentViewModel>()
                .ForMember(dest => dest.UserFullName,
                    opt =>
                        opt.MapFrom<PostCommentViewModelUserFullNameResolver>());

            CreateMap<PostCommentFormViewModel, PostComment>();
        }
    }
}
