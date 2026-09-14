namespace CareConnect.DTOs.Visits;

public sealed class CreateVisitDto
{
    public required int CaregiverAssignmentId { get; init; }

    public required DateTime ScheduledStartUtc { get; init; }

    public required DateTime ScheduledEndUtc { get; init; }

    /// <summary>Seeds the visit's task checklist at creation time.</summary>
    public required IReadOnlyList<int> CareTaskIds { get; init; }
}
