using LMS.Common.CQRS;

namespace LMS.Users.Application.Commands;

public record UpdateUserCommand(
    Guid UserId,
    string? Bio,
    Guid? AvatarMediaId) : ICommand;
