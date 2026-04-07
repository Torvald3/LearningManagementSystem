namespace LMS.Users.Api.Models;

public record LoginResponse(string AccessToken, DateTime ExpiresAtUtc);