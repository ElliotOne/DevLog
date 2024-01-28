using DevLog.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevLog.Data.Configurations
{
    public class SettingConfiguration : IEntityTypeConfiguration<Setting>
    {
        public void Configure(EntityTypeBuilder<Setting> builder)
        {
            builder
                .Property(x => x.Phone1)
                .HasMaxLength(20);

            builder
                .Property(x => x.Phone2)
                .HasMaxLength(20);

            builder
                .Property(x => x.Email1)
                .HasMaxLength(256);

            builder
                .Property(x => x.Email2)
                .HasMaxLength(256);

            builder
                .Property(x => x.Instagram)
                .HasMaxLength(256);

            builder
                .Property(x => x.Telegram)
                .HasMaxLength(256);

            builder
                .Property(x => x.GooglePlus)
                .HasMaxLength(256);

            builder
                .Property(x => x.FaceBook)
                .HasMaxLength(256);

            builder
                .Property(x => x.LinkedIn)
                .HasMaxLength(256);

            builder
                .Property(x => x.YouTube)
                .HasMaxLength(256);

            builder
                .Property(x => x.GitHub)
                .HasMaxLength(256);

            builder
                .Property(x => x.WalletName1)
                .HasMaxLength(256);

            builder
                .Property(x => x.WalletAddress1)
                .HasMaxLength(1000);

            builder
                .Property(x => x.WalletName2)
                .HasMaxLength(256);

            builder
                .Property(x => x.WalletAddress2)
                .HasMaxLength(1000);

            builder
                .Property(x => x.WalletName3)
                .HasMaxLength(256);

            builder
                .Property(x => x.WalletAddress3)
                .HasMaxLength(1000);

            builder
                .Property(x => x.WalletName4)
                .HasMaxLength(256);

            builder
                .Property(x => x.WalletAddress4)
                .HasMaxLength(1000);
        }
    }
}
