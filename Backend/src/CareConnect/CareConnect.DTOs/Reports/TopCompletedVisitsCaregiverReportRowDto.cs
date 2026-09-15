namespace CareConnect.DTOs.Reports;

public sealed class TopCompletedVisitsCaregiverReportRowDto
{
    public required int Rank { get; init; }

    public required int CaregiverId { get; init; }

    public required string CaregiverFullName { get; init; }

    public required int CompletedVisitCount { get; init; }
}
