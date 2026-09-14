namespace CareConnect.DTOs.Caregivers;

public sealed class CreateCaregiverAvailabilityDto
{
    public required int CaregiverId { get; init; }

    public required DayOfWeek DayOfWeek { get; init; }

    public required TimeOnly StartTime { get; init; }

    public required TimeOnly EndTime { get; init; }
}
