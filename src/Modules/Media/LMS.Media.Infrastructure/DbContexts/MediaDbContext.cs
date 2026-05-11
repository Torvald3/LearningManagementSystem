using LMS.Media.Infrastructure.Entities;
using LMS.Media.Infrastructure.EntityTypeConfigurations;
using Microsoft.EntityFrameworkCore;

namespace LMS.Media.Infrastructure.DbContexts;

public class MediaDbContext : DbContext
{
    public DbSet<MediaFile> MediaFiles => Set<MediaFile>();

    public MediaDbContext(DbContextOptions<MediaDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("media");
        modelBuilder.ApplyConfiguration(new MediaFileConfiguration());
    }
}
