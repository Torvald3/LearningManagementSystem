using LMS.Common.CQRS;
using LMS.Identity.Api.Models;
using LMS.Identity.Application.Commands.ConfirmEmail;
using LMS.Identity.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
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

    private static async Task<Results<NoContent, BadRequest<IEnumerable<string>>, NotFound<string>>> ConfirmEmailAsync(
        ConfirmEmailRequest request,
        ICommandHandler<ConfirmEmailCommand, ConfirmEmailResult> commandHandler)
    {
        var result = await commandHandler.HandleAsync(new ConfirmEmailCommand(request.UserId, request.Token));

        return result.Status switch
        {
            ConfirmEmailStatus.Success => TypedResults.NoContent(),
            ConfirmEmailStatus.UserNotFound => TypedResults.NotFound($"User with id {request.UserId} not found"),
            _ => TypedResults.BadRequest(result.Errors)
        };
    }
}