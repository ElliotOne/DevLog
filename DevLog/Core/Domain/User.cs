using Microsoft.AspNetCore.Identity;

namespace DevLog.Core.Domain
{
    public class User : IdentityUser<int>
    {
        public override string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public override string Email { get; set; } = string.Empty;
        public string? Biography { get; set; }
        public bool IsActive { get; set; }
        public string? ImageVirtualPath { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastEditDate { get; set; }
        public ICollection<Post>? Posts { get; set; }
        public ICollection<PostComment>? PostComments { get; set; }
    }
}
