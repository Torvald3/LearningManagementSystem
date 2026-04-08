using LMS.Users.Api.Extensions;
using LMS.Courses.Api.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.App.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddModulesServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddUsersModuleServices(configuration);
        services.AddCoursesModuleServices(configuration);

        return services;
    }
}