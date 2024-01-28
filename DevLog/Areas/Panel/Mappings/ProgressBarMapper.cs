using AutoMapper;
using DevLog.Areas.Panel.Models.ViewModels;
using DevLog.Core.Domain;

namespace DevLog.Areas.Panel.Mappings
{
    public class ProgressBarMapper : Profile
    {
        public ProgressBarMapper()
        {
            CreateMap<ProgressBar, ProgressBarFormViewModel>();
            CreateMap<ProgressBarFormViewModel, ProgressBar>();
        }
    }
}
