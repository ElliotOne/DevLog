using AutoMapper;
using DevLog.Areas.Panel.Models.ViewModels;
using DevLog.Core.Domain;

namespace DevLog.Areas.Panel.Mappings
{
    public class CertificateMapper : Profile
    {
        public CertificateMapper()
        {
            CreateMap<Certificate, CertificateIndexViewModel>();
            CreateMap<Certificate, CertificateFormViewModel>();
            CreateMap<CertificateFormViewModel, Certificate>();
        }
    }
}
