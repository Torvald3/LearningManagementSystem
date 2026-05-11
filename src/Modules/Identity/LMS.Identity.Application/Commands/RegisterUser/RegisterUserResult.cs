namespace LMS.Identity.Application.Commands.RegisterUser;

public record RegisterUserResult(
    Guid UserId,
    string Email,
    string ConfirmationToken);
