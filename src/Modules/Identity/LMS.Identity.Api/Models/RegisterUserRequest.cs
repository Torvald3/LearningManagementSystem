namespace LMS.Identity.Api.Models;

public record RegisterUserRequest(
    string Email,
    string Password,
    string Username);