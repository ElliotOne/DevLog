using DevLog.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevLog.Data.Configurations
{
    public class PostCommentConfiguration : IEntityTypeConfiguration<PostComment>
    {
        public void Configure(EntityTypeBuilder<PostComment> builder)
        {
            builder
                .HasOne(x => x.Post)
                .WithMany(x => x.PostComments)
                .HasForeignKey(x => x.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Property(x => x.UserFullName)
                .HasMaxLength(256);

            builder
                .Property(x => x.Email)
                .HasMaxLength(256);

            builder
                .Property(x => x.Body)
                .HasMaxLength(1024)
                .IsRequired();

            builder
                .Property(x => x.Ip)
                .HasMaxLength(256)
                .IsRequired();
        }
    }
}
