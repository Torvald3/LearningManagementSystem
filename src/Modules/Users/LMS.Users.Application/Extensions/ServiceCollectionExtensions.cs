using LMS.Common.CQRS;
using LMS.Users.Application.Commands;
using LMS.Users.Application.Queries;
using LMS.Users.Application.Services;
using LMS.Users.Contracts.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.Users.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateUserCommand>, CreateUserCommandHandler>();

        services.AddScoped<IQueryHandler<UserExistsQuery, bool>, UserExistsQueryHandler>(); 
        
        services.AddScoped<IUsersModuleService, UsersModuleService>();
        
        return services;
    }
}