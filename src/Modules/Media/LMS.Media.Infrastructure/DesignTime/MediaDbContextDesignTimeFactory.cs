using LMS.Common.Database.DesignTime;
using LMS.Media.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace LMS.Media.Infrastructure.DesignTime;

public class MediaDbContextDesignTimeFactory : DesignTimeDbContextFactoryBase<MediaDbContext>
{
    public MediaDbContextDesignTimeFactory()
        : base("LMS.Media.Infrastructure", "media")
    {
    }

    protected override MediaDbContext CreateNewInstance(DbContextOptions<MediaDbContext> options)
    {
        return new MediaDbContext(options);
    }

    protected override string GetConnectionString()
    {
        return DatabaseConfiguration.ConnectionString;
    }
}
