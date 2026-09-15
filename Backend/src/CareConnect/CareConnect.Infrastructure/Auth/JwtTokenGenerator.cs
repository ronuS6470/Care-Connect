using System.Text;
using CareConnect.Infrastructure.Entities;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace CareConnect.Infrastructure.Auth;

/// <summary>An issued access token and the instant it stops being valid.</summary>
public readonly record struct IssuedToken(string Token, DateTime ExpiresAtUtc);

public interface IJwtTokenGenerator
{
    IssuedToken Generate(User user);
}

/// <summary>
/// Issues the API's own HS256 access tokens.
///
/// Two claims are load-bearing and must not be renamed casually:
///   "sub"  — set to <see cref="User.Auth0UserId"/>, because every handler resolves the caller
///            through ICurrentUserAccessor, which reads exactly this claim and looks the user up
///            by that column.
///   "role" — what [Authorize(Roles = "...")] checks, via RoleClaimType = "role" configured
///            alongside token validation in InfrastructureServiceCollectionExtensions.
/// </summary>
public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtOptions _options;

    public JwtTokenGenerator(JwtOptions options)
    {
        _options = options;
    }

    public IssuedToken Generate(User user)
    {
        var issuedAt = DateTime.UtcNow;
        var expiresAt = issuedAt.AddMinutes(_options.AccessTokenMinutes);

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            IssuedAt = issuedAt,
            NotBefore = issuedAt,
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
            Claims = new Dictionary<string, object>
            {
                ["sub"] = user.Auth0UserId,
                ["role"] = user.Role.ToString(),
                ["email"] = user.Email,
                ["name"] = $"{user.FirstName} {user.LastName}",
            },
        };

        return new IssuedToken(new JsonWebTokenHandler().CreateToken(descriptor), expiresAt);
    }
}
