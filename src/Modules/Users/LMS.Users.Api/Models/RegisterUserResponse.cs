namespace LMS.Users.Api.Models;

public record RegisterUserResponse(
    Guid UserId,
    string Email,
    string UserName,
    string? ConfirmationToken);