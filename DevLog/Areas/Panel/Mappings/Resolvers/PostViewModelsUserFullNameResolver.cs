using AutoMapper;
using DevLog.Core.Domain;

namespace DevLog.Areas.Panel.Mappings.Resolvers
{
    internal class PostViewModelsUserFullNameResolver<T>
        : IValueResolver<Post, T, string>
    {
        public string Resolve(
            Post source, T destination, string destMember, ResolutionContext context)
        {
            return source.User.FirstName + " " + source.User.LastName;
        }
    }
}
