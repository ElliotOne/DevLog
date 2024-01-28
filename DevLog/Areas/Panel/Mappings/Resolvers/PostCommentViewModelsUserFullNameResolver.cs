using AutoMapper;
using DevLog.Core.Domain;

namespace DevLog.Areas.Panel.Mappings.Resolvers
{
    internal class PostCommentViewModelsUserFullNameResolver<T>
        : IValueResolver<PostComment, T, string>
    {
        public string Resolve(
            PostComment source,
            T destination,
            string destMember,
            ResolutionContext context)
        {
            return (source.User == null ? source.UserFullName : source.User.FirstName + " " + source.User.LastName)
                   ?? string.Empty;
        }
    }
}
