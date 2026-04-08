using LMS.Common.Database.Configuration;
using LMS.Courses.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.Courses.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoursesInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var databaseConfiguration = configuration
            .GetRequiredSection("Database")
            .Get<DatabaseConfiguration>()!;

        services.AddDbContext<CoursesDbContext>(options =>
            options.UseNpgsql(databaseConfiguration.ConnectionString));

        return services;
    }
}