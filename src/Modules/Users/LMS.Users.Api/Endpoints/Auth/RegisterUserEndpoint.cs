using LMS.Users.Api.Models;
using LMS.Users.Core.Models;
using LMS.Users.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;

namespace LMS.Users.Api.Endpoints.Auth;

internal static class RegisterUserEndpoint
{
    public static RouteGroupBuilder MapRegisterUser(this RouteGroupBuilder group)
    {
        group.MapGet("/register", RegisterUserAsync)
            .WithName($"{nameof(RegisterUserAsync)}");
         
        return group;
    }
    
    private static async Task<Results<Ok<RegisterUserResponse>, BadRequest<IEnumerable<IdentityError>>>> RegisterUserAsync(
        RegisterUserRequest request,
        UserManager<ApplicationUser> userManager,
        IEmailService emailService)
    {
        var user = new ApplicationUser()
        {
            UserName =  request.Username,
            Email =  request.Email,
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return TypedResults.BadRequest(result.Errors);
        }
        
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        await emailService.SendUserConfirmationEmailAsync(user.Email, token);

        //TODO: do not return confirmationToken in production 
        return TypedResults.Ok(new RegisterUserResponse(user.Id, user.Email, user.UserName, token));
    }
}