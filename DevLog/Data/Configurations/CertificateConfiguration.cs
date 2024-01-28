using DevLog.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevLog.Data.Configurations
{
    public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
    {
        public void Configure(EntityTypeBuilder<Certificate> builder)
        {
            builder
                .Property(x => x.Title)
                .HasMaxLength(256)
                .IsRequired();

            builder
                .Property(x => x.Url)
                .HasMaxLength(1000);
        }
    }
}
