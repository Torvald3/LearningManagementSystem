namespace LMS.Identity.Application.Commands.LoginUser;

public record LoginUserResult(
    bool Succeeded,
    string? AccessToken,
    DateTime? ExpiresAtUtc,
    LoginError Error);