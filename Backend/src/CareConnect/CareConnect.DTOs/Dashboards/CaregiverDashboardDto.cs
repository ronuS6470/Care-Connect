using CareConnect.DTOs.Reporting;

namespace CareConnect.DTOs.Dashboards;

/// <summary>
/// A caregiver's own dashboard — always scoped to the authenticated caregiver, never accepts a
/// caregiverId from the caller.
///
/// ActualEarnings is computed the same way as CaregiverEarningsCalculator (actual check-in/out ×
/// current HourlyRate) over every completed visit, all-time. EstimatedUpcomingEarnings is
/// necessarily an estimate — it projects from each Scheduled visit's *scheduled* duration (actual
/// times don't exist yet), so it will differ from what's actually earned once those visits happen.
/// </summary>
public sealed class CaregiverDashboardDto
{
    public required IReadOnlyList<VisitSummaryDto> TodaysVisits { get; init; }

    public required IReadOnlyList<VisitSummaryDto> UpcomingVisits { get; init; }

    public required int CompletedVisitCount { get; init; }

    public required decimal TotalHoursWorked { get; init; }

    public required decimal ActualEarnings { get; init; }

    public required decimal EstimatedUpcomingEarnings { get; init; }
}
