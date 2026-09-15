namespace CareConnect.DTOs.Reports;

public sealed class CaregiverWithoutVisitsTodayDto
{
    public required int CaregiverId { get; init; }

    public required string CaregiverFullName { get; init; }
}
