using CareConnect.DTOs.Enums;

namespace CareConnect.Queries.Security;

/// <summary>The authenticated caller's identity, resolved from their Auth0 "sub" claim.</summary>
public sealed class RequesterContext
{
    public required int UserId { get; init; }

    public required UserRole Role { get; init; }

    public int? ClientId { get; init; }

    public int? CaregiverId { get; init; }
}
