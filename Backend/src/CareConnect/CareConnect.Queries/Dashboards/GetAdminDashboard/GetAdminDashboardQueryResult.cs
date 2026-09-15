namespace CareConnect.Queries.Dashboards.GetAdminDashboard;

/// <summary>Result set 3 of GetAdminDashboardQuery.sql.</summary>
internal sealed class TodaysBreakdownRow
{
    public int TodaysVisitCount { get; init; }

    public int CompletedVisitsToday { get; init; }

    public int PendingVisitsToday { get; init; }

    public int CancelledVisitsToday { get; init; }
}

/// <summary>Result set 4 of GetAdminDashboardQuery.sql.</summary>
internal sealed class HoursEarningsRow
{
    public decimal TotalHoursWorked { get; init; }

    public decimal TotalCaregiverEarnings { get; init; }
}
