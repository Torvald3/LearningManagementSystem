using LMS.Common.CQRS;
using LMS.Identity.Api.Models;
using LMS.Identity.Application.Commands.LoginUser;
using LMS.Identity.Application.Errors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace LMS.Identity.Api.Endpoints;

public static class LoginEndpoint
{
    public static RouteGroupBuilder MapLoginEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/login", LoginAsync)
             .WithName($"{nameof(LoginAsync)}");
         
        return group;
    }

    private static async Task<Results<Ok<LoginResponse>, UnauthorizedHttpResult, BadRequest<string>>> LoginAsync(
        LoginRequest request,
        ICommandHandler<LoginUserCommand, LoginUserResult> commandHandler)
    {
        var result = await commandHandler.HandleAsync(new LoginUserCommand(request.Email, request.Password));

        if (result.IsFailure)
        {
            if (result.Error.Code == IdentityErrors.EmailNotConfirmed.Code)
            {
                return TypedResults.BadRequest(result.Error.Message);
            }

            return TypedResults.Unauthorized();
        }

        var loginResult = result.Value;

        return TypedResults.Ok(new LoginResponse(
            loginResult.AccessToken,
            loginResult.ExpiresAtUtc));
    }
}
