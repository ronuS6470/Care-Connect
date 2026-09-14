namespace CareConnect.DTOs.Caregivers;

public sealed class UpdateCaregiverDto
{
    public string? LicenseNumber { get; init; }

    public required decimal HourlyRate { get; init; }

    public required DateOnly HireDate { get; init; }

    public required int YearsOfExperience { get; init; }

    public required bool IsActive { get; init; }
}
