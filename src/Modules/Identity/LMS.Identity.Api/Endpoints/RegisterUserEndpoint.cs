using LMS.Common.CQRS;
using LMS.Common.Observability.Logging;
using LMS.Identity.Api.Models;
using LMS.Identity.Application.Commands;
using LMS.Identity.Application.Commands.RegisterUser;
using LMS.Identity.Core.Models;
using LMS.Identity.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace LMS.Identity.Api.Endpoints;

internal static class RegisterUserEndpoint
{
    public static RouteGroupBuilder MapRegisterUser(this RouteGroupBuilder group)
    {
        group.MapPost("/register", RegisterUserAsync)
             .WithName($"{nameof(RegisterUserAsync)}");
         
        return group;
    }

    private static async Task<Results<Ok<RegisterUserResponse>, BadRequest<IEnumerable<string>>>> RegisterUserAsync(
        RegisterUserRequest request,
        ICommandHandler<RegisterUserCommand, RegisterUserResult> handler)
    {
        var result = await handler.HandleAsync(new RegisterUserCommand(request.Email, request.Password, request.Username));

        if (!result.Succeeded)
        {
            IEnumerable<string> errors = result.Errors;
            return TypedResults.BadRequest(errors);
        }

        return TypedResults.Ok(new RegisterUserResponse(result.UserId!.Value, result.Email!, result.ConfirmationToken));
    }
}