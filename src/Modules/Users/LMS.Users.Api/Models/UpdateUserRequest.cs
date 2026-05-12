namespace LMS.Users.Api.Models;

public record UpdateUserRequest(
    string? Bio,
    Guid? AvatarMediaId);
