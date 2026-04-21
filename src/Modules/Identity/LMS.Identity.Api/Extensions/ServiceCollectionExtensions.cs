using System.Text;
using LMS.Common.Database.Configuration;
using LMS.Identity.Application.Extensions;
using LMS.Identity.Core.Configurations;
using LMS.Identity.Core.Models;
using LMS.Identity.Infrastructure.DbContexts;
using LMS.Identity.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace LMS.Identity.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityModuleServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityCore<ApplicationUser>(opts =>
            {
                opts.User.RequireUniqueEmail = true;
                opts.SignIn.RequireConfirmedEmail = true;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();
        
        services.AddApiServices()
                .AddApplicationServices()
                .AddInfrastructureServices(configuration.GetRequiredSection("Database").Get<DatabaseConfiguration>()!);

        return services;
    }

    public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSection = configuration.GetRequiredSection("JwtAuth");
        
        services.AddOptions<JwtAuthConfiguration>()
                .Bind(jwtSection);
        
        var jwtConfiguration = jwtSection.Get<JwtAuthConfiguration>();
        
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtConfiguration!.Issuer,
                    ValidAudience = jwtConfiguration.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.SigningKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });
        
        services.AddAuthorization();

        return services;
    }

    private static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        return services;
    }
}