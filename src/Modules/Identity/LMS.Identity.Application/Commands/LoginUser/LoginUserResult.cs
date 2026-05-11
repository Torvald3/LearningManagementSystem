namespace LMS.Identity.Application.Commands.LoginUser;

public record LoginUserResult(
    string AccessToken,
    DateTime ExpiresAtUtc);
