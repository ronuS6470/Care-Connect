using CareConnect.DTOs.Enums;

namespace CareConnect.DTOs.Reporting;

/// <summary>Lightweight row shape for visit lists/reports — <see cref="Visits.VisitDto"/> is the full detail view.</summary>
public sealed class VisitSummaryDto
{
    public required int VisitId { get; init; }

    public required string CaregiverFullName { get; init; }

    public required string ClientFullName { get; init; }

    public required DateTime ScheduledStartUtc { get; init; }

    public required DateTime ScheduledEndUtc { get; init; }

    public required VisitStatus Status { get; init; }
}
