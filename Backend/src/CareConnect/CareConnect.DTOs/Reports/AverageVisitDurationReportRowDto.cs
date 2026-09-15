namespace CareConnect.DTOs.Reports;

public sealed class AverageVisitDurationReportRowDto
{
    public required int CaregiverId { get; init; }

    public required string CaregiverFullName { get; init; }

    public required int CompletedVisitCount { get; init; }

    public required decimal AverageVisitDurationHours { get; init; }
}
