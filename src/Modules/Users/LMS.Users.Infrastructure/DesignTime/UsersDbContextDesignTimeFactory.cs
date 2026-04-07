using LMS.Users.Infrastructure.DbContexts;

namespace LMS.Users.Infrastructure.DesignTime;

using Microsoft.EntityFrameworkCore;

public class UsersDbContextDesignTimeFactory : DesignTimeDbContextFactoryBase<UsersDbContext>
{
    public UsersDbContextDesignTimeFactory()
        : base("LMS.Users.Infrastructure")
    {
    }

    protected override UsersDbContext CreateNewInstance(DbContextOptions<UsersDbContext> options)
    {
        return new UsersDbContext(options);
    }

    protected override string GetConnectionString()
    {
        return DatabaseConfiguration.ConnectionString;
    }
}
