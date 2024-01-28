using AutoMapper;
using DevLog.Areas.Panel.Mappings.Resolvers;
using DevLog.Areas.Panel.Models.ViewModels;
using DevLog.Core.Domain;

namespace DevLog.Areas.Panel.Mappings
{
    public class PostCommentMapper : Profile
    {
        public PostCommentMapper()
        {
            CreateMap<PostComment, PostCommentIndexViewModel>()
                .ForMember(dest => dest.UserFullName,
                    opt =>
                        opt.MapFrom<PostCommentViewModelsUserFullNameResolver<PostCommentIndexViewModel>>());

            CreateMap<PostComment, PostCommentFormViewModel>()
                .ForMember(dest => dest.UserFullName,
                    opt =>
                        opt.MapFrom<PostCommentViewModelsUserFullNameResolver<PostCommentFormViewModel>>());

            CreateMap<PostCommentFormViewModel, PostComment>();

            CreateMap<PostComment, PostCommentReplyFormViewModel>()
                .ForMember(dest => dest.ParentId,
                    opt =>
                        opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ParentBody,
                    opt =>
                        opt.MapFrom(src => src.Body))
                .ForMember(dest => dest.Body,
                    opt =>
                        opt.MapFrom(_ => string.Empty));

            CreateMap<PostCommentReplyFormViewModel, PostComment>();
        }
    }
}
