using AutoMapper;
using DevLog.Core.Domain;
using DevLog.Models.ViewModels;

namespace DevLog.Mappings
{
    public class CertificateMapper : Profile
    {
        public CertificateMapper()
        {
            CreateMap<Certificate, CertificateViewModel>();
        }
    }
}
