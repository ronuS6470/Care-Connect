namespace CareConnect.DTOs.Reports;

public sealed class VisitsPerCaregiverReportRowDto
{
    public required int CaregiverId { get; init; }

    public required string CaregiverFullName { get; init; }

    public required int TotalVisits { get; init; }

    public required int CompletedVisits { get; init; }

    public required int CancelledVisits { get; init; }

    public required int NoShowVisits { get; init; }
}
