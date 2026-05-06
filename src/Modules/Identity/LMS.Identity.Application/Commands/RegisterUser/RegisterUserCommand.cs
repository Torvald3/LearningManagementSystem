using LMS.Common.CQRS;

namespace LMS.Identity.Application.Commands.RegisterUser;

public record RegisterUserCommand(
    string Email,
    string Password, 
    string Username) : ICommand;