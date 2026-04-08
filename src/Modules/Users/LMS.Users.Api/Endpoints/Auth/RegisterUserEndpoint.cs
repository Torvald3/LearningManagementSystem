using LMS.Users.Api.Models;
using LMS.Users.Core.Models;
using LMS.Users.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using LMS.Common.Observability.Logging;

namespace LMS.Users.Api.Endpoints.Auth;

internal static class RegisterUserEndpoint
{
    public static RouteGroupBuilder MapRegisterUser(this RouteGroupBuilder group)
    {
        group.MapPost("/register", RegisterUserAsync)
             .WithName($"{nameof(RegisterUserAsync)}");
         
        return group;
    }

    private static async Task<Results<Ok<RegisterUserResponse>, BadRequest<IEnumerable<IdentityError>>>> RegisterUserAsync(
    RegisterUserRequest request,
    UserManager<ApplicationUser> userManager,
    IEmailService emailService,
    ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("Users.Register");
        var maskedEmail = PiiMaskingHelper.MaskEmail(request.Email);

        logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} email={Email}",
            DateTime.UtcNow,
            "INFO",
            "user.register.requested",
            maskedEmail);

        var user = new ApplicationUser()
        {
            UserName = request.Username,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            logger.LogWarning(
                "timestamp={Timestamp} level={Level} event={Event} email={Email} errors_count={ErrorsCount}",
                DateTime.UtcNow,
                "WARN",
                "user.register.failed",
                maskedEmail,
                result.Errors.Count());

            return TypedResults.BadRequest(result.Errors);
        }

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        await emailService.SendUserConfirmationEmailAsync(user.Email!, token);

        logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} user_id={UserId}",
            DateTime.UtcNow,
            "INFO",
            "user.register.succeeded",
            user.Id);

        return TypedResults.Ok(new RegisterUserResponse(user.Id, user.Email!, user.UserName!, token));
    }
}