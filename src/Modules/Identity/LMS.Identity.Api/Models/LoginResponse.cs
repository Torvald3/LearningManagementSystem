namespace LMS.Identity.Api.Models;

public record LoginResponse(string AccessToken, DateTime ExpiresAtUtc);