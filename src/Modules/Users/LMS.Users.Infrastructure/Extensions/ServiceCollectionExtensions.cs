using LMS.Common.Database.Configuration;
using LMS.Users.Core.Services;
using LMS.Users.Infrastructure.DbContexts;
using LMS.Users.Infrastructure.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.Users.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, DatabaseConfiguration databaseConfiguration)
    {
        services.AddDbContext<UsersDbContext>(options =>
        {
            options.UseNpgsql(databaseConfiguration.ConnectionString);
        });

        services.AddScoped<IUsersService, UsersService>();
        
        return services;
    }
}