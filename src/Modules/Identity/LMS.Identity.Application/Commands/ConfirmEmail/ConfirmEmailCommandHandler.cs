using LMS.Common.CQRS;
using LMS.Common.Results;
using LMS.Identity.Application.Errors;
using LMS.Identity.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace LMS.Identity.Application.Commands.ConfirmEmail;

public class ConfirmEmailCommandHandler : ICommandHandler<ConfirmEmailCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ConfirmEmailCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> HandleAsync(
        ConfirmEmailCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(command.UserId.ToString());

        if (user is null)
        {
            return IdentityErrors.UserNotFound(command.UserId);
        }
        
        if (user.EmailConfirmed)
        {
            return IdentityErrors.EmailAlreadyConfirmed;
        }

        var result = await _userManager.ConfirmEmailAsync(user, command.Token);

        if (!result.Succeeded)
        {
            return IdentityErrors.InvalidEmailConfirmationToken(result.Errors.Select(e => e.Description));
        }

        return Result.Success;
    }
}
