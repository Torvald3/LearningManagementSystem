using System.Runtime.CompilerServices;
using LMS.Users.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace LMS.Users.Infrastructure.DbContexts;

public class UsersDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    
    public UsersDbContext(DbContextOptions<UsersDbContext> options) 
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("users");

        builder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);
    }
}