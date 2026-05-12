using LMS.Courses.Infrastructure.Entities;
using LMS.Courses.Infrastructure.EntityTypeConfigurations;
using Microsoft.EntityFrameworkCore;

namespace LMS.Courses.Infrastructure.DbContexts;

public class CoursesDbContext : DbContext
{
    public DbSet<Course> Courses => Set<Course>();

    public DbSet<CourseModule> CourseModules => Set<CourseModule>();

    public DbSet<Lesson> Lessons => Set<Lesson>();

    public DbSet<CourseMember> CourseMembers => Set<CourseMember>();
    
    public CoursesDbContext(DbContextOptions<CoursesDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasDefaultSchema("courses");

        modelBuilder.ApplyConfiguration(new CourseConfiguration());
        modelBuilder.ApplyConfiguration(new CourseMemberConfiguration());
        modelBuilder.ApplyConfiguration(new CourseModuleConfiguration());
        modelBuilder.ApplyConfiguration(new LessonConfiguration());
    }
}
