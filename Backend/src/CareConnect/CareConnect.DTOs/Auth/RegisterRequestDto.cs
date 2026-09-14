using CareConnect.DTOs.Enums;

namespace CareConnect.DTOs.Auth;

public sealed class RegisterRequestDto
{
    public required string Email { get; init; }

    public required string Password { get; init; }

    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public string? PhoneNumber { get; init; }

    public required UserRole Role { get; init; }
}
