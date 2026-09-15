namespace CareConnect.DTOs.Reports;

/// <summary>
/// One row is one conflict. ConflictType is "OutsideAvailability" (the visit no longer falls
/// within any of the caregiver's current active availability windows — typically because
/// availability was edited after the visit was scheduled) or "OverlappingVisits" (two of the
/// caregiver's own visits overlap in time — shouldn't happen through the API's normal scheduling
/// path, but is worth surfacing if data was changed directly). ConflictingVisitId and Details are
/// only populated for OverlappingVisits.
/// </summary>
public sealed class CaregiverAvailabilityConflictDto
{
    public required string ConflictType { get; init; }

    public required int CaregiverId { get; init; }

    public required string CaregiverFullName { get; init; }

    public required int VisitId { get; init; }

    public required DateTime ScheduledStartUtc { get; init; }

    public required DateTime ScheduledEndUtc { get; init; }

    public int? ConflictingVisitId { get; init; }

    public string? Details { get; init; }
}
