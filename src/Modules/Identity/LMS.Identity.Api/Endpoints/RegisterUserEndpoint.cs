using LMS.Common.CQRS;
using LMS.Identity.Api.Models;
using LMS.Identity.Application.Commands.RegisterUser;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

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

        if (result.IsFailure)
        {
            IEnumerable<string> errors = new[] { result.Error.Message };
            return TypedResults.BadRequest(errors);
        }

        var registerUserResult = result.Value;

        return TypedResults.Ok(new RegisterUserResponse(
            registerUserResult.UserId,
            registerUserResult.Email,
            registerUserResult.ConfirmationToken));
    }
}
