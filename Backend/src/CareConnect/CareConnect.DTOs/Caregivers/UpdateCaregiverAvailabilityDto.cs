namespace CareConnect.DTOs.Caregivers;

public sealed class UpdateCaregiverAvailabilityDto
{
    public required DayOfWeek DayOfWeek { get; init; }

    public required TimeOnly StartTime { get; init; }

    public required TimeOnly EndTime { get; init; }

    public required bool IsActive { get; init; }
}
