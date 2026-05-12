namespace LMS.Users.Application.Models;

public record User(
    Guid Id,
    string Username,
    string Email,
    string? Bio,
    Guid? AvatarMediaId);
