using AutoMapper;
using DevLog.Areas.Panel.Models.ViewModels;
using DevLog.Core.Domain;

namespace DevLog.Areas.Panel.Mappings.Resolvers
{
    internal class UserIndexViewModelUserFullNameResolver
        : IValueResolver<User, UserIndexViewModel, string>
    {
        public string Resolve(
            User source, UserIndexViewModel destination, string destMember, ResolutionContext context)
        {
            return source.FirstName + " " + source.LastName;
        }
    }
}
