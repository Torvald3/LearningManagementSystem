using LMS.Common.CQRS;
using LMS.Identity.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace LMS.Identity.Application.Commands.ConfirmEmail;

public class ConfirmEmailCommandHandler : ICommandHandler<ConfirmEmailCommand, ConfirmEmailResult>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ConfirmEmailCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ConfirmEmailResult> HandleAsync(ConfirmEmailCommand command, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(command.UserId.ToString());

        if (user is null)
        {
            return new ConfirmEmailResult(ConfirmEmailStatus.UserNotFound, [ $"User with id {command.UserId} not found"] );
        }
        
        if (user.EmailConfirmed)
        {
            return new ConfirmEmailResult(ConfirmEmailStatus.AlreadyConfirmed, [ "Email is already confirmed." ]);
        }

        var result = await _userManager.ConfirmEmailAsync(user, command.Token);

        if (!result.Succeeded)
        {
            return new ConfirmEmailResult(ConfirmEmailStatus.InvalidToken, result.Errors.Select(e => e.Description));
        }

        return new ConfirmEmailResult(ConfirmEmailStatus.Success, []);
    }
}