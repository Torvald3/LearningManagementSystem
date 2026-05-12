using LMS.Courses.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Courses.Infrastructure.EntityTypeConfigurations;

public class CourseMemberConfiguration : IEntityTypeConfiguration<CourseMember>
{
    public void Configure(EntityTypeBuilder<CourseMember> builder)
    {
        builder.ToTable("course_member", "courses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CourseId)
               .IsRequired();

        builder.Property(x => x.UserId)
               .IsRequired();

        builder.Property(x => x.Role)
               .HasConversion<string>()
               .HasMaxLength(32)
               .IsRequired();

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.UpdatedAt)
               .IsRequired();

        builder.HasOne(x => x.Course)
               .WithMany(x => x.Members)
               .HasForeignKey(x => x.CourseId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.CourseId);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => new { x.CourseId, x.UserId })
               .IsUnique();
        builder.HasIndex(x => x.CourseId)
               .IsUnique()
               .HasDatabaseName("IX_course_member_CourseId_CourseOwner")
               .HasFilter("\"Role\" = 'CourseOwner'");
        builder.HasIndex(x => new { x.CourseId, x.Role });
    }
}
