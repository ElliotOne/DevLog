using DevLog.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace DevLog.Data
{
    /// <summary>
    /// Represents application database context interface
    /// </summary>
    public interface IApplicationDbContext
    {
        DbSet<Certificate> Certificates { get; set; }
        DbSet<Contact> Contacts { get; set; }
        DbSet<Post> Posts { get; set; }
        DbSet<PostCategory> PostCategories { get; set; }
        DbSet<PostComment> PostComments { get; set; }
        DbSet<ProgressBar> ProgressBars { get; set; }
        DbSet<Setting> Settings { get; set; }
        DbSet<User> Users { get; set; }
    }
}
