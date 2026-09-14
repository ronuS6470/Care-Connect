namespace CareConnect.Infrastructure.Auth;

/// <summary>Binds the "Auth0" configuration section used to validate incoming JWTs.</summary>
public sealed class Auth0Options
{
    public const string SectionName = "Auth0";

    public required string Domain { get; init; }

    public required string Audience { get; init; }
}
