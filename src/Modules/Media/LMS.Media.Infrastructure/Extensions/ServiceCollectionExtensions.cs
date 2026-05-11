using LMS.Common.Database.Configuration;
using LMS.Media.Core.Configurations;
using LMS.Media.Core.Services;
using LMS.Media.Infrastructure.DbContexts;
using LMS.Media.Infrastructure.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace LMS.Media.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        DatabaseConfiguration databaseConfiguration,
        MediaStorageConfiguration storageConfiguration)
    {
        services.AddDbContext<MediaDbContext>(options => options.UseNpgsql(databaseConfiguration.ConnectionString));

        services.AddSingleton(storageConfiguration);
        services.AddSingleton<IMinioClient>(_ => new MinioClient()
            .WithEndpoint(storageConfiguration.Endpoint)
            .WithCredentials(storageConfiguration.AccessKey, storageConfiguration.SecretKey)
            .WithSSL(storageConfiguration.UseSsl)
            .Build());

        services.AddScoped<IMediaService, MediaService>();
        services.AddScoped<IMediaStorage, MinioMediaStorage>();

        return services;
    }
}
