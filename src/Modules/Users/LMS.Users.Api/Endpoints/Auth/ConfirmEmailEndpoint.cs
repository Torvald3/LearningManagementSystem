using LMS.Users.Api.Models;
using LMS.Users.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;

namespace LMS.Users.Api.Endpoints.Auth;

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
        UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            return TypedResults.NotFound($"User with id {request.UserId} not found");
        }
        
        if (user.EmailConfirmed)
        {
            IEnumerable<string> errors = new List<string> { "Email is already confirmed." };
            return TypedResults.BadRequest(errors);
        }

        var result = await userManager.ConfirmEmailAsync(user, request.Token);

        if (!result.Succeeded)
        {
            return TypedResults.BadRequest(result.Errors.Select(e => e.Description));
        }

        return TypedResults.NoContent();
    }
}