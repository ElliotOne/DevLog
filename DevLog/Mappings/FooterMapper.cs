using AutoMapper;
using DevLog.Core.Domain;
using DevLog.Models.ViewModels;

namespace DevLog.Mappings
{
    public class FooterMapper : Profile
    {
        public FooterMapper()
        {
            CreateMap<Setting, FooterViewModel>();
        }
    }
}
