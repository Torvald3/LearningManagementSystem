using LMS.Common.CQRS;
using LMS.Common.Results;
using LMS.Users.Application.Errors;
using LMS.Users.Application.Models;
using LMS.Users.Core.Services;
using CoreUser = LMS.Users.Core.Models.User;

namespace LMS.Users.Application.Commands;

public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, User>
{
    private const int BioMaxLength = 1024;

    private readonly IUsersService _usersService;

    public UpdateUserCommandHandler(IUsersService usersService)
    {
        _usersService = usersService;
    }

    public async Task<Result<User>> HandleAsync(
        UpdateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.Bio is not null && command.Bio.Length > BioMaxLength)
        {
            return UserErrors.BioTooLong(BioMaxLength);
        }

        if (command.AvatarMediaId == Guid.Empty)
        {
            return UserErrors.AvatarMediaIdInvalid;
        }

        var existingUser = await _usersService.GetUserAsync(command.UserId, cancellationToken);

        if (existingUser is null)
        {
            return UserErrors.UserNotFound(command.UserId);
        }

        var bio = string.IsNullOrWhiteSpace(command.Bio)
            ? null
            : command.Bio.Trim();

        var updatedUser = new CoreUser
        {
            Id = existingUser.Id,
            Username = existingUser.Username,
            Bio = bio,
            AvatarMediaId = command.AvatarMediaId,
            Contacts = existingUser.Contacts
        };

        var updated = await _usersService.UpdateUserAsync(updatedUser, cancellationToken);

        if (!updated)
        {
            return UserErrors.UserNotFound(command.UserId);
        }

        return new User(
            updatedUser.Id,
            updatedUser.Username,
            updatedUser.Contacts.Email,
            updatedUser.Bio,
            updatedUser.AvatarMediaId);
    }
}
