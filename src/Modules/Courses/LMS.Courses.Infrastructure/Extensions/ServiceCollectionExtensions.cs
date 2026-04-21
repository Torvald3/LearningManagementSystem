using LMS.Common.Database.Configuration;
using LMS.Courses.Core.Services;
using LMS.Courses.Infrastructure.DbContexts;
using LMS.Courses.Infrastructure.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.Courses.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, DatabaseConfiguration configuration)
    {
        services.AddDbContext<CoursesDbContext>(options => options.UseNpgsql(configuration.ConnectionString));

        services.AddScoped<ICoursesService, CoursesService>();
        
        return services;
    }
}