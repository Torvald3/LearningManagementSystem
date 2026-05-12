namespace LMS.Users.Api.Models;

public record UserResponse(
    Guid Id,
    string Username,
    string Email,
    string? Bio,
    Guid? AvatarMediaId);
