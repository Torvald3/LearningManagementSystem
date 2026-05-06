using LMS.Courses.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Courses.Infrastructure.EntityTypeConfigurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("course", "courses");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(x => x.Theme)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.Description)
               .IsRequired()
               .HasMaxLength(2000);

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.UpdatedAt)
               .IsRequired();
        
        builder.Property(x => x.AuthorId)
               .IsRequired();
    }
}