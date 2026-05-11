using LMS.Courses.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Courses.Infrastructure.EntityTypeConfigurations;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.ToTable("lesson", "courses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(x => x.Content)
               .IsRequired()
               .HasMaxLength(10000);

        builder.Property(x => x.Position)
               .IsRequired();

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.UpdatedAt)
               .IsRequired();

        builder.Property(x => x.IsArchived)
               .IsRequired();

        builder.Property(x => x.ArchivedAt);

        builder.Property(x => x.ModuleId)
               .IsRequired();

        builder.HasOne(x => x.Module)
               .WithMany(x => x.Lessons)
               .HasForeignKey(x => x.ModuleId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ModuleId);
        builder.HasIndex(x => new { x.ModuleId, x.IsArchived });
        builder.HasIndex(x => new { x.ModuleId, x.IsArchived, x.Position });
    }
}
