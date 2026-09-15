namespace CareConnect.Infrastructure.Auth;

/// <summary>
/// Binds the "Jwt" configuration section. This API issues and validates its own tokens, so the
/// same values are used on both sides — <see cref="SigningKey"/> is a shared secret and must come
/// from user-secrets, an environment variable, or a key vault outside local development. Never
/// commit a production key.
/// </summary>
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public required string Issuer { get; init; }

    public required string Audience { get; init; }

    /// <summary>HMAC-SHA256 signing secret; must be at least 32 bytes or token creation throws.</summary>
    public required string SigningKey { get; init; }

    public int AccessTokenMinutes { get; init; } = 60;
}
