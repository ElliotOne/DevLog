using DevLog.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevLog.Data.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder
                .Property(x => x.Title)
                .HasMaxLength(256)
                .IsRequired();

            builder
                .Property(x => x.Body)
                .IsRequired();

            builder
                .Property(x => x.Tags)
                .HasMaxLength(1024);
        }
    }
}
