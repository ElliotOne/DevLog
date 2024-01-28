using DevLog.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevLog.Data.Configurations
{
    public class ProgressBarConfiguration : IEntityTypeConfiguration<ProgressBar>
    {
        public void Configure(EntityTypeBuilder<ProgressBar> builder)
        {
            builder
                .Property(x => x.Topic)
                .HasMaxLength(256)
                .IsRequired();
        }
    }
}
