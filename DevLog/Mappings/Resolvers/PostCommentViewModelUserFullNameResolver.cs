using AutoMapper;
using DevLog.Core.Domain;
using DevLog.Models.ViewModels;

namespace DevLog.Mappings.Resolvers
{
    internal class PostCommentViewModelUserFullNameResolver
        : IValueResolver<PostComment, PostCommentViewModel, string?>
    {
        public string? Resolve(
            PostComment source, PostCommentViewModel destination, string? destMember, ResolutionContext context)
        {
            return source.User == null ? source.UserFullName : source.User.FirstName + " " + source.User.LastName;
        }
    }
}
