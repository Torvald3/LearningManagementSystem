using LMS.Common.CQRS;

namespace LMS.Identity.Application.Commands.ConfirmEmail;

public record ConfirmEmailCommand(Guid UserId, string Token) : ICommand;