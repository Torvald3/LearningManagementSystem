using LMS.Common.Database.Configuration;
using LMS.Media.Application.Extensions;
using LMS.Media.Core.Configurations;
using LMS.Media.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.Media.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediaModuleServices(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseConfiguration = configuration.GetSection("Database").Get<DatabaseConfiguration>();
        var storageConfiguration = configuration.GetSection("MediaStorage").Get<MediaStorageConfiguration>();

        services.AddApplicationServices()
                .AddInfrastructureServices(databaseConfiguration!, storageConfiguration!);

        return services;
    }
}
