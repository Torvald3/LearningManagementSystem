using System.Text;
using LMS.Common.Database.Configuration;
using LMS.Users.Api.Configurations;
using LMS.Users.Application.Extensions;
using LMS.Users.Core.Models;
using LMS.Users.Infrastructure.DbContexts;
using LMS.Users.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace LMS.Users.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUsersModuleServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<UsersDbContext>();
        
        services.AddApiServices()
                .AddApplicationServices()
                .AddInfrastructureServices(configuration.GetValue<DatabaseConfiguration>("Database")!);

        return services;
    }

    public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtConfiguration = configuration.GetRequiredSection("JwtAuth");
        
        services.AddOptions<JwtAuthConfiguration>()
                .Bind(jwtConfiguration);
        
        // services
        //     .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //     .AddJwtBearer(options =>
        //     {
        //         options.TokenValidationParameters = new TokenValidationParameters
        //         {
        //             ValidateIssuer = true,
        //             ValidateAudience = true,
        //             ValidateIssuerSigningKey = true,
        //             ValidateLifetime = true,
        //             ValidIssuer = jwtConfiguration.,
        //             ValidAudience = jwtConfiguration.Audience,
        //             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.SigningKey)),
        //             ClockSkew = TimeSpan.Zero
        //         };
        //     });
        //
        // builder.Services.AddAuthorization();

        return services;
    }

    private static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        return services;
    }
}