using System.Reflection.Emit;
using LMS.Courses.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS.Courses.Infrastructure.DbContexts;

public class CoursesDbContext : DbContext
{
    public CoursesDbContext(DbContextOptions<CoursesDbContext> options)
        : base(options)
    {
    }

    public DbSet<Course> Courses => Set<Course>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.Theme)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(2000);

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.Property(x => x.UpdatedAt)
                .IsRequired();
        });
    }
}