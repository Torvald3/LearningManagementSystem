using LMS.Common.Database.Configuration;
using LMS.Users.Application.Extensions;
using LMS.Users.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.Users.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUsersModuleServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationServices()
                .AddInfrastructureServices(configuration.GetRequiredSection("Database").Get<DatabaseConfiguration>()!);
        
        return services;
    }
}