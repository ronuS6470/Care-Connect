namespace CareConnect.Queries.Dashboards.GetCaregiverDashboard;

/// <summary>Result set 3 of GetCaregiverDashboardQuery.sql.</summary>
internal sealed class CaregiverCompletedHoursRow
{
    public int CompletedVisitCount { get; init; }

    public decimal TotalHoursWorked { get; init; }

    public decimal ActualEarnings { get; init; }
}
