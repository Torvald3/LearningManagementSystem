using LMS.Common.Database.Configuration;
using LMS.Identity.Core.Services;
using LMS.Identity.Core.TokenGenerator;
using LMS.Identity.Infrastructure.DbContexts;
using LMS.Identity.Infrastructure.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.Identity.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, DatabaseConfiguration databaseConfiguration)
    {
        services.AddDbContext<IdentityDbContext>(options =>
        {
            options.UseNpgsql(databaseConfiguration.ConnectionString);
        });

        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IAccessTokenGenerator, AccessTokenGenerator>();
        
        return services;
    }
}