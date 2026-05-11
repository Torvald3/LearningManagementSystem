using LMS.Common.CQRS;

namespace LMS.Identity.Application.Commands.LoginUser;

public record LoginUserCommand(string Email, string Password) : ICommand;