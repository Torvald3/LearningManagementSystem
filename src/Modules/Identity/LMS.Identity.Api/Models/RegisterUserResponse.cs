namespace LMS.Identity.Api.Models;

public record RegisterUserResponse(
    Guid UserId,
    string Email,
    string? ConfirmationToken);