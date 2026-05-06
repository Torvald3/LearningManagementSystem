namespace LMS.Identity.Core.TokenGenerator;

public record AccessTokenResult(string Token, DateTime ExpiresAt);