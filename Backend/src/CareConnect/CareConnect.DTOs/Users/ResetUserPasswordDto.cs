namespace CareConnect.DTOs.Users;

/// <summary>
/// An Admin setting another account's password. Deliberately does not carry the current password —
/// that is the whole point of a reset, and why the endpoint is Admin-only.
/// </summary>
public sealed class ResetUserPasswordDto
{
    public required string NewPassword { get; init; }
}
