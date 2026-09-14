namespace CareConnect.DTOs.Caregivers;

public sealed class CaregiverAvailabilityDto
{
    public required int Id { get; init; }

    public required int CaregiverId { get; init; }

    public required DayOfWeek DayOfWeek { get; init; }

    public required TimeOnly StartTime { get; init; }

    public required TimeOnly EndTime { get; init; }

    public required bool IsActive { get; init; }
}
