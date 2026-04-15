namespace LMS.Users.Contracts.Models;

public record CreateUserRequest(
    Guid UserId,
    string Email,
    string Username);