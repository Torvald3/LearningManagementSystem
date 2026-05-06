using System.Security.Claims;
using System.Text;
using LMS.Common.CQRS;
using LMS.Common.Observability.Logging;
using LMS.Common.Observability.Metrics;
using LMS.Identity.Core.Configurations;
using LMS.Identity.Core.Models;
using LMS.Identity.Core.TokenGenerator;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LMS.Identity.Application.Commands.LoginUser;

public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, LoginUserResult>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppMetrics _metrics;
    private readonly IAccessTokenGenerator _accessTokenGenerator;
    
    private readonly ILogger<LoginUserCommandHandler> _logger;

    public LoginUserCommandHandler(
        UserManager<ApplicationUser> userManager, 
        AppMetrics metrics, 
        ILogger<LoginUserCommandHandler> logger, 
        IAccessTokenGenerator accessTokenGenerator)
    {
        _userManager = userManager;
        _metrics = metrics;
        _logger = logger;
        _accessTokenGenerator = accessTokenGenerator;
    }

    public async Task<LoginUserResult> HandleAsync(LoginUserCommand command, CancellationToken cancellationToken = default)
    {
        var safeLogin = PiiMaskingHelper.MaskEmail(command.Email);
        
        _logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} login={Login}",
            DateTime.UtcNow,
            "INFO",
            "user.login.requested",
            safeLogin);

        var user = await _userManager.FindByEmailAsync(command.Email);

        if (user is null)
        {
            _logger.LogWarning(
                "timestamp={Timestamp} level={Level} event={Event} login={Login}",
                DateTime.UtcNow,
                "WARN",
                "user.login.failed.user_not_found",
                safeLogin);

            return new LoginUserResult(false, null, null, LoginError.InvalidCredentials);
        }

        if (!user.EmailConfirmed)
        {
            _logger.LogWarning(
                "timestamp={Timestamp} level={Level} event={Event} user_id={UserId}",
                DateTime.UtcNow,
                "WARN",
                "user.login.failed.email_not_confirmed",
                user.Id);

            return new LoginUserResult(false, null, null, LoginError.EmailNotConfirmed);
        }

        var isValidPassword = await _userManager.CheckPasswordAsync(user, command.Password);

        if (!isValidPassword)
        {
            _logger.LogWarning(
                "timestamp={Timestamp} level={Level} event={Event} user_id={UserId}",
                DateTime.UtcNow,
                "WARN",
                "user.login.failed.invalid_password",
                user.Id);

            return new LoginUserResult(false, null, null, LoginError.InvalidCredentials);
        }

        var accessTokenResult = _accessTokenGenerator.Generate(user);

        _metrics.SessionOpened();

        _logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} user_id={UserId}",
            DateTime.UtcNow,
            "INFO",
            "user.login.succeeded",
            user.Id);

        return new LoginUserResult(true, accessTokenResult.Token, accessTokenResult.ExpiresAt, LoginError.None);
    }
}