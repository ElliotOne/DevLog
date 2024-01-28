using AutoMapper;
using DevLog.Areas.Panel.Models.ViewModels;
using DevLog.Core.Domain;

namespace DevLog.Areas.Panel.Mappings
{
    public class SettingMapper : Profile
    {
        public SettingMapper()
        {
            CreateMap<Setting, SettingFormViewModel>();
            CreateMap<SettingFormViewModel, Setting>();
        }
    }
}
