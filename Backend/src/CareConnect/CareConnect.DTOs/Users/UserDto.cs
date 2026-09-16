using CareConnect.DTOs.Enums;

namespace CareConnect.DTOs.Users;

/// <summary>
/// An account as the admin user-management screen sees it. CaregiverId/ClientId say whether the
/// user actually has the profile their role implies: a user can hold Role=Caregiver with no
/// Caregivers row, because creating that profile requires the role to be set first.
/// </summary>
public sealed class UserDto
{
    public required int Id { get; init; }

    public required string Email { get; init; }

    public required string FullName { get; init; }

    public string? PhoneNumber { get; init; }

    public required UserRole Role { get; init; }

    public required bool IsActive { get; init; }

    /// <summary>Whether this account can sign in at all — an account with no password never can.</summary>
    public required bool HasPassword { get; init; }

    public int? CaregiverId { get; init; }

    public int? ClientId { get; init; }

    public required DateTime CreatedAtUtc { get; init; }
}
