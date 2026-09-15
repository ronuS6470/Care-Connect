namespace CareConnect.DTOs.Reports;

public sealed class CareTaskDemandReportRowDto
{
    public required int CareTaskId { get; init; }

    public required string CareTaskName { get; init; }

    public required int TimesRequested { get; init; }

    public required int TimesCompleted { get; init; }
}
