namespace CareConnect.DTOs.Caregivers;

public sealed class CaregiverDto
{
    public required int Id { get; init; }

    public required int UserId { get; init; }

    public required string FullName { get; init; }

    public required string Email { get; init; }

    public string? PhoneNumber { get; init; }

    public string? LicenseNumber { get; init; }

    public required decimal HourlyRate { get; init; }

    public required DateOnly HireDate { get; init; }

    public required DateOnly DateOfBirth { get; init; }

    public required int YearsOfExperience { get; init; }

    public required bool IsActive { get; init; }
}
