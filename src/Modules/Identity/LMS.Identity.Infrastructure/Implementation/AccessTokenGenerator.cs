using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LMS.Identity.Core.Configurations;
using LMS.Identity.Core.Models;
using LMS.Identity.Core.TokenGenerator;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace LMS.Identity.Infrastructure.Implementation;

public class AccessTokenGenerator : IAccessTokenGenerator
{
    private readonly JwtAuthConfiguration _jwtOptions;

    public AccessTokenGenerator(IOptions<JwtAuthConfiguration> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public AccessTokenResult Generate(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenLifetimeInMinutes);

        var jwt = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);
        
        return new AccessTokenResult(accessToken, expiresAtUtc);
    }
}