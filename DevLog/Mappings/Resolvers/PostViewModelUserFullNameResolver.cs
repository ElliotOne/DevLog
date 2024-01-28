using AutoMapper;
using DevLog.Core.Domain;
using DevLog.Models.ViewModels;

namespace DevLog.Mappings.Resolvers
{
    internal class PostViewModelUserFullNameResolver
        : IValueResolver<Post, PostViewModel, string>
    {
        public string Resolve(
            Post source, PostViewModel destination, string destMember, ResolutionContext context)
        {
            return source.User.FirstName + " " + source.User.LastName;
        }
    }
}
