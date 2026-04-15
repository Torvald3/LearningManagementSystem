namespace LMS.Identity.Application.Commands.RegisterUser;

public record RegisterUserResult(
    bool Succeeded,
    Guid? UserId,
    string? Email,
    string? ConfirmationToken,
    List<string> Errors);