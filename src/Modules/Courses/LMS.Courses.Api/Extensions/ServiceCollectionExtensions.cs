using LMS.Courses.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.Courses.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoursesModuleServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCoursesInfrastructure(configuration);

        return services;
    }
}