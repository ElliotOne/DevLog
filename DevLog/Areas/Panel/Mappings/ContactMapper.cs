using AutoMapper;
using DevLog.Areas.Panel.Models.ViewModels;
using DevLog.Core.Domain;

namespace DevLog.Areas.Panel.Mappings
{
    public class ContactMapper : Profile
    {
        public ContactMapper()
        {
            CreateMap<Contact, ContactIndexViewModel>();
            CreateMap<Contact, ContactFormViewModel>();
            CreateMap<ContactFormViewModel, Contact>();
        }
    }
}
