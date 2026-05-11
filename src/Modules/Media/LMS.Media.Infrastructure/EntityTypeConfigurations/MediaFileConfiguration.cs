using LMS.Media.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Media.Infrastructure.EntityTypeConfigurations;

public class MediaFileConfiguration : IEntityTypeConfiguration<MediaFile>
{
    public void Configure(EntityTypeBuilder<MediaFile> builder)
    {
        builder.ToTable("media_file", "media");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EntityType)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.Property(x => x.EntityId)
               .IsRequired();

        builder.Property(x => x.ObjectKey)
               .IsRequired()
               .HasMaxLength(1000);

        builder.Property(x => x.OriginalFileName)
               .IsRequired()
               .HasMaxLength(255);

        builder.Property(x => x.ContentType)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.Size)
               .IsRequired();

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.IsArchived)
               .IsRequired();

        builder.Property(x => x.ArchivedAt);

        builder.HasIndex(x => new { x.EntityType, x.EntityId, x.IsArchived });
        builder.HasIndex(x => x.ObjectKey)
               .IsUnique();
    }
}
