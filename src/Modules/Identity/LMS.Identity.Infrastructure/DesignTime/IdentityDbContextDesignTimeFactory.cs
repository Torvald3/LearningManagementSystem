using LMS.Identity.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace LMS.Identity.Infrastructure.DesignTime;

public class IdentityDbContextDesignTimeFactory : DesignTimeDbContextFactoryBase<IdentityDbContext>
{
    public IdentityDbContextDesignTimeFactory()
        : base("LMS.Identity.Infrastructure")
    {
    }

    protected override IdentityDbContext CreateNewInstance(DbContextOptions<IdentityDbContext> options)
    {
        return new IdentityDbContext(options);
    }

    protected override string GetConnectionString()
    {
        return DatabaseConfiguration.ConnectionString;
    }
}