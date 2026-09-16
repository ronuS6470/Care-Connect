namespace CareConnect.DTOs.Auth;

/// <summary>A signed-in user changing their own password. Always requires the current one.</summary>
public sealed class ChangePasswordDto
{
    public required string CurrentPassword { get; init; }

    public required string NewPassword { get; init; }
}
