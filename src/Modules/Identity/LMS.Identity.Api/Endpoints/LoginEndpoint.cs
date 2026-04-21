using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LMS.Common.CQRS;
using LMS.Common.Observability.Logging;
using LMS.Common.Observability.Metrics;
using LMS.Identity.Api.Models;
using LMS.Identity.Application.Commands.LoginUser;
using LMS.Identity.Core.Configurations;
using LMS.Identity.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

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

        if (!result.Succeeded)
        {
            if (result.Error == LoginError.EmailNotConfirmed)
            {
                return TypedResults.BadRequest("Email is not confirmed.");
            }

            return TypedResults.Unauthorized();
        }

        return TypedResults.Ok(new LoginResponse(result.AccessToken!, result.ExpiresAtUtc!.Value));
    }
}