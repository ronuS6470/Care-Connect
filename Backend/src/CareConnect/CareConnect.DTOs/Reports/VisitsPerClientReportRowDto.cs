namespace CareConnect.DTOs.Reports;

public sealed class VisitsPerClientReportRowDto
{
    public required int ClientId { get; init; }

    public required string ClientFullName { get; init; }

    public required int TotalVisits { get; init; }

    public required int CompletedVisits { get; init; }

    public required int CancelledVisits { get; init; }

    public required int NoShowVisits { get; init; }
}
