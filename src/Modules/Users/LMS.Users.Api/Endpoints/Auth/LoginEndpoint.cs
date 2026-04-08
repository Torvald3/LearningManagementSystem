using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LMS.Users.Api.Configurations;
using LMS.Users.Api.Models;
using LMS.Users.Core.Models;
using LMS.Users.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using LMS.Common.Observability.Logging;
using LMS.Common.Observability.Metrics;

namespace LMS.Users.Api.Endpoints.Auth;

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
    UserManager<ApplicationUser> userManager,
    IOptions<JwtAuthConfiguration> jwtOptions,
    AppMetrics metrics,
    ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("Users.Login");
        var safeLogin = PiiMaskingHelper.MaskEmail(request.EmailOrUsername);

        logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} login={Login}",
            DateTime.UtcNow,
            "INFO",
            "user.login.requested",
            safeLogin);

        var user = await userManager.FindByEmailAsync(request.EmailOrUsername);

        if (user is null)
        {
            user = await userManager.FindByNameAsync(request.EmailOrUsername);

            if (user is null)
            {
                logger.LogWarning(
                    "timestamp={Timestamp} level={Level} event={Event} login={Login}",
                    DateTime.UtcNow,
                    "WARN",
                    "user.login.failed.user_not_found",
                    safeLogin);

                return TypedResults.Unauthorized();
            }
        }

        if (!user.EmailConfirmed)
        {
            logger.LogWarning(
                "timestamp={Timestamp} level={Level} event={Event} user_id={UserId}",
                DateTime.UtcNow,
                "WARN",
                "user.login.failed.email_not_confirmed",
                user.Id);

            return TypedResults.BadRequest("Email is not confirmed.");
        }

        var isValidPassword = await userManager.CheckPasswordAsync(user, request.Password);

        if (!isValidPassword)
        {
            logger.LogWarning(
                "timestamp={Timestamp} level={Level} event={Event} user_id={UserId}",
                DateTime.UtcNow,
                "WARN",
                "user.login.failed.invalid_password",
                user.Id);

            return TypedResults.Unauthorized();
        }

        var claims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
        new(ClaimTypes.Email, user.Email ?? string.Empty)
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(jwtOptions.Value.AccessTokenLifetimeInMinutes);

        var jwt = new JwtSecurityToken(
            issuer: jwtOptions.Value.Issuer,
            audience: jwtOptions.Value.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        metrics.SessionOpened();

        logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} user_id={UserId}",
            DateTime.UtcNow,
            "INFO",
            "user.login.succeeded",
            user.Id);

        return TypedResults.Ok(new LoginResponse(accessToken, expiresAtUtc));
    }
}