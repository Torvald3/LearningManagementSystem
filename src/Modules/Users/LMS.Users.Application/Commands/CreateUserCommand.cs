using LMS.Common.CQRS;

namespace LMS.Users.Application.Commands;

public record CreateUserCommand(
    Guid UserId,
    string Email,
    string Username) : ICommand;