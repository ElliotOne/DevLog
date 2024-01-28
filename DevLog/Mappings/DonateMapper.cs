using AutoMapper;
using DevLog.Core.Domain;
using DevLog.Models.ViewModels;

namespace DevLog.Mappings
{
    public class DonateMapper : Profile
    {
        public DonateMapper()
        {
            CreateMap<Setting, DonateViewModel>();
        }
    }
}
