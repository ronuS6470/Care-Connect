namespace CareConnect.DTOs.Reporting;

public sealed class DashboardStatisticsDto
{
    public required int TotalActiveCaregivers { get; init; }

    public required int TotalActiveClients { get; init; }

    public required int ActiveAssignments { get; init; }

    public required int VisitsScheduledToday { get; init; }

    public required int VisitsCompletedToday { get; init; }

    public required int VisitsCancelledOrNoShowToday { get; init; }

    public required int PendingInvoiceCount { get; init; }

    public required decimal PendingInvoiceTotal { get; init; }
}
