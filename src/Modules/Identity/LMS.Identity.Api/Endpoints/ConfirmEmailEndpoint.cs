using LMS.Common.CQRS;
using LMS.Common.Results;
using LMS.Identity.Api.Models;
using LMS.Identity.Application.Commands.ConfirmEmail;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace LMS.Identity.Api.Endpoints;

internal static class ConfirmEmailEndpoint
{
    public static RouteGroupBuilder MapConfirmEmail(this RouteGroupBuilder group)
    {
        group.MapPost("/confirm-email", ConfirmEmailAsync)
             .WithName($"{nameof(ConfirmEmailAsync)}");
         
        return group;
    }

    private static async Task<Results<NoContent, BadRequest<IEnumerable<string>>, NotFound<string>, Conflict<string>>> ConfirmEmailAsync(
        ConfirmEmailRequest request,
        ICommandHandler<ConfirmEmailCommand> commandHandler)
    {
        var result = await commandHandler.HandleAsync(new ConfirmEmailCommand(request.UserId, request.Token));

        if (result.IsSuccess)
        {
            return TypedResults.NoContent();
        }

        return result.Error.Type switch
        {
            ErrorType.NotFound => TypedResults.NotFound(result.Error.Message),
            ErrorType.Conflict => TypedResults.Conflict(result.Error.Message),
            _ => BadRequest(result.Error)
        };
    }

    private static BadRequest<IEnumerable<string>> BadRequest(Error error)
    {
        IEnumerable<string> errors = new[] { error.Message };
        return TypedResults.BadRequest(errors);
    }
}
