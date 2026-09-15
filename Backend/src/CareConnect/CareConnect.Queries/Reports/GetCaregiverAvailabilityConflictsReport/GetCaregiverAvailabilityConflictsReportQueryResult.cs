namespace CareConnect.Queries.Reports.GetCaregiverAvailabilityConflictsReport;

/// <summary>Result set 1 of GetCaregiverAvailabilityConflictsReportQuery.sql.</summary>
internal sealed class ActiveVisitRow
{
    public int VisitId { get; init; }

    public int CaregiverId { get; init; }

    public string CaregiverFullName { get; init; } = string.Empty;

    public DateTime ScheduledStartUtc { get; init; }

    public DateTime ScheduledEndUtc { get; init; }
}

/// <summary>Result set 2 of GetCaregiverAvailabilityConflictsReportQuery.sql.</summary>
internal sealed class OverlapRow
{
    public int CaregiverId { get; init; }

    public string CaregiverFullName { get; init; } = string.Empty;

    public int VisitId { get; init; }

    public DateTime ScheduledStartUtc { get; init; }

    public DateTime ScheduledEndUtc { get; init; }

    public int ConflictingVisitId { get; init; }
}

/// <summary>Result set 3 of GetCaregiverAvailabilityConflictsReportQuery.sql.</summary>
internal sealed class AvailabilityWindowRow
{
    public int CaregiverId { get; init; }

    public DayOfWeek DayOfWeek { get; init; }

    public TimeOnly StartTime { get; init; }

    public TimeOnly EndTime { get; init; }
}
