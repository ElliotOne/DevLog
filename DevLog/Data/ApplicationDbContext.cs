using DevLog.Core.Domain;
using DevLog.Data.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DevLog.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, int>, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(CertificateConfiguration).Assembly);
            base.OnModelCreating(builder);
        }

        public DbSet<User> ApplicationUser { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostCategory> PostCategories { get; set; }
        public DbSet<PostComment> PostComments { get; set; }
        public DbSet<ProgressBar> ProgressBars { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Contact> Contacts { get; set; }
    }
}
