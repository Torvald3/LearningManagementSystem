using LMS.Common.Database.DesignTime;
using LMS.Users.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace LMS.Users.Infrastructure.DesignTime;

public class UsersDbContextDesignTimeFactory : DesignTimeDbContextFactoryBase<UsersDbContext>
{
    public UsersDbContextDesignTimeFactory()
        : base("LMS.Users.Infrastructure", "users")
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