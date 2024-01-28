using AutoMapper;
using DevLog.Core.Domain;
using DevLog.Models.ViewModels;

namespace DevLog.Mappings
{
    public class ContactMapper : Profile
    {
        public ContactMapper()
        {
            CreateMap<ContactFormViewModel, Contact>();
        }
    }
}
