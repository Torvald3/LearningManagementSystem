using LMS.Common.CQRS;
using LMS.Identity.Application.Commands;
using LMS.Identity.Application.Commands.RegisterUser;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.Identity.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<RegisterUserCommand, RegisterUserResult>, RegisterUserCommandHandler>();
        
        return services;
    }
}