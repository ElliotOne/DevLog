using DevLog.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevLog.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
                .Property(x => x.UserName)
                .HasMaxLength(256)
                .IsRequired();

            builder
                .Property(x => x.FirstName)
                .HasMaxLength(256)
                .IsRequired();

            builder
                .Property(x => x.LastName)
                .HasMaxLength(256)
                .IsRequired();

            builder
                .Property(x => x.Biography)
                .HasMaxLength(1024);

            builder
                .Property(x => x.Email)
                .HasMaxLength(256)
                .IsRequired();
        }
    }
}
