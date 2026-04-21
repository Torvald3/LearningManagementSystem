using LMS.Courses.Infrastructure.Entities;
using LMS.Courses.Infrastructure.EntityTypeConfigurations;
using Microsoft.EntityFrameworkCore;

namespace LMS.Courses.Infrastructure.DbContexts;

public class CoursesDbContext : DbContext
{
    public DbSet<Course> Courses => Set<Course>();
    
    public CoursesDbContext(DbContextOptions<CoursesDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasDefaultSchema("courses");

        modelBuilder.ApplyConfiguration(new CourseConfiguration());
    }
}