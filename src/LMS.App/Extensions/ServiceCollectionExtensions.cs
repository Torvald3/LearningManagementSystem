using LMS.Users.Api.Extensions;

namespace LMS.App.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddModulesServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddUsersModuleServices(configuration);
        
        return services;
    }
}