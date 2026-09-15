namespace CareConnect.DTOs.Dashboards;

/// <summary>
/// Org-wide snapshot for the Admin dashboard.
///
/// Scope notes (documented here since the source fields don't carry qualifiers of their own):
/// - TotalClients counts every Client row (active and inactive); ActiveCaregivers is filtered to
///   IsActive caregivers only, since an inactive caregiver isn't operationally relevant today.
/// - TodaysVisitCount/CompletedVisitsToday/PendingVisitsToday/CancelledVisitsToday are all scoped
///   to visits whose ScheduledStartUtc falls on today's date (UTC) — they're a single "how's today
///   going" breakdown, not all-time counts.
/// - TotalHoursWorked/TotalCaregiverEarnings are all-time totals across every completed visit for
///   every caregiver, computed from actual check-in/check-out time (never the scheduled window),
///   at each caregiver's current HourlyRate — same rule as CaregiverEarningsCalculator, aggregated
///   in SQL here for performance rather than replayed per-visit in C#.
/// </summary>
public sealed class AdminDashboardDto
{
    public required int TotalClients { get; init; }

    public required int ActiveCaregivers { get; init; }

    public required int TodaysVisitCount { get; init; }

    public required int CompletedVisitsToday { get; init; }

    public required int PendingVisitsToday { get; init; }

    public required int CancelledVisitsToday { get; init; }

    public required decimal TotalHoursWorked { get; init; }

    public required decimal TotalCaregiverEarnings { get; init; }
}
