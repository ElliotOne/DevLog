using DevLog.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevLog.Data.Configurations
{
    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder
                .Property(x => x.UserFullName)
                .HasMaxLength(256)
                .IsRequired();

            builder
                .Property(x => x.EmailOrPhoneNumber)
                .HasMaxLength(256)
                .IsRequired();

            builder
                .Property(x => x.Subject)
                .HasMaxLength(256)
                .IsRequired();

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
