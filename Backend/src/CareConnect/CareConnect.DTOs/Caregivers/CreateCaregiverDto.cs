namespace CareConnect.DTOs.Caregivers;

/// <summary>Attaches a caregiver profile to an existing user (created via registration/Auth0 sync).</summary>
public sealed class CreateCaregiverDto
{
    public required int UserId { get; init; }

    public string? LicenseNumber { get; init; }

    public required decimal HourlyRate { get; init; }

    public required DateOnly HireDate { get; init; }

    public required DateOnly DateOfBirth { get; init; }

    public required int YearsOfExperience { get; init; }
}
