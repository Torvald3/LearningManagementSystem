namespace LMS.Identity.Core.Configurations;

public class JwtAuthConfiguration
{
    public string Issuer { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    public string SigningKey { get; init; } = string.Empty;

    public int AccessTokenLifetimeInMinutes { get; init; } = 15;

    public int RefreshTokenLifetimeInDays { get; init; } = 30;
}