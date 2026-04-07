namespace LMS.Users.Api.Models;

public record RegisterUserRequest(
    string Email,
    string Password,
    string Username);