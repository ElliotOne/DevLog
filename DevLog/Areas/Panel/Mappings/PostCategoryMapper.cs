using AutoMapper;
using DevLog.Areas.Panel.Models.ViewModels;
using DevLog.Core.Domain;

namespace DevLog.Areas.Panel.Mappings
{
    public class PostCategoryMapper : Profile
    {
        public PostCategoryMapper()
        {
            CreateMap<PostCategory, PostCategoryIndexViewModel>();
            CreateMap<PostCategory, PostCategoryFormViewModel>();
            CreateMap<PostCategoryFormViewModel, PostCategory>();
        }
    }
}
