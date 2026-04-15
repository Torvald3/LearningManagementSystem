using LMS.Common.CQRS;
using LMS.Common.Observability.Logging;
using LMS.Identity.Core.Models;
using LMS.Identity.Core.Services;
using LMS.Users.Contracts.Models;
using LMS.Users.Contracts.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LMS.Identity.Application.Commands.RegisterUser;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly ILogger<RegisterUserCommandHandler> _logger;
    private readonly IUsersModuleService _usersModuleService;

    public RegisterUserCommandHandler(
        UserManager<ApplicationUser> userManager, 
        IEmailService emailService, 
        IUsersModuleService usersModuleService,
        ILogger<RegisterUserCommandHandler> logger)
    {
        _userManager = userManager;
        _emailService = emailService;
        _logger = logger;
        _usersModuleService = usersModuleService;
    }

    public async Task<RegisterUserResult> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        var maskedEmail = PiiMaskingHelper.MaskEmail(command.Email);

        _logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} email={Email}",
            DateTime.UtcNow,
            "INFO",
            "user.register.requested",
            maskedEmail);

        var user = new ApplicationUser()
        {
            Email = command.Email
        };

        var result = await _userManager.CreateAsync(user, command.Password);

        if (!result.Succeeded)
        {
            _logger.LogWarning(
                "timestamp={Timestamp} level={Level} event={Event} email={Email} errors_count={ErrorsCount}",
                DateTime.UtcNow,
                "WARN",
                "user.register.failed",
                maskedEmail,
                result.Errors.Count());

            return new RegisterUserResult(false, null, null, null, result.Errors.Select(e => e.Description).ToList());
        }
        
        await _usersModuleService.CreateUserAsync(new CreateUserRequest(user.Id, command.Email, command.Username));
        
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        await _emailService.SendUserConfirmationEmailAsync(user.Email!, token);
        
        _logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} user_id={UserId}",
            DateTime.UtcNow,
            "INFO",
            "user.register.succeeded",
            user.Id);

        return new RegisterUserResult(true, user.Id, user.Email, token, []);
    }
}