using LMS.Courses.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Courses.Infrastructure.EntityTypeConfigurations;

public class CourseModuleConfiguration : IEntityTypeConfiguration<CourseModule>
{
    public void Configure(EntityTypeBuilder<CourseModule> builder)
    {
        builder.ToTable("course_module", "courses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(x => x.Description)
               .IsRequired()
               .HasMaxLength(2000);

        builder.Property(x => x.Position)
               .IsRequired();

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.UpdatedAt)
               .IsRequired();

        builder.Property(x => x.IsArchived)
               .IsRequired();

        builder.Property(x => x.ArchivedAt);

        builder.Property(x => x.CourseId)
               .IsRequired();

        builder.HasOne(x => x.Course)
               .WithMany(x => x.Modules)
               .HasForeignKey(x => x.CourseId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.CourseId);
        builder.HasIndex(x => new { x.CourseId, x.IsArchived });
        builder.HasIndex(x => new { x.CourseId, x.IsArchived, x.Position });
    }
}
