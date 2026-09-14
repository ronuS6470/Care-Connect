using CareConnect.DTOs.Enums;

namespace CareConnect.DTOs.Auth;

public sealed class LoginResponseDto
{
    public required string Token { get; init; }

    public required int UserId { get; init; }

    public required string FullName { get; init; }

    public required string Email { get; init; }

    public required UserRole Role { get; init; }

    public required DateTime ExpiresAtUtc { get; init; }
}
