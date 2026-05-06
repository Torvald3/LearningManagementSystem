using LMS.Users.Api.Extensions;
using LMS.Courses.Api.Extensions;
using LMS.Identity.Api.Extensions;

namespace LMS.App.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddModulesServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityModuleServices(configuration);
        
        services.AddUsersModuleServices(configuration);
        services.AddCoursesModuleServices(configuration);

        return services;
    }
}