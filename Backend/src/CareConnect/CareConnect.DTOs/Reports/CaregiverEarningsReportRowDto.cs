namespace CareConnect.DTOs.Reports;

public sealed class CaregiverEarningsReportRowDto
{
    public required int CaregiverId { get; init; }

    public required string CaregiverFullName { get; init; }

    public required decimal HourlyRate { get; init; }

    public required decimal TotalHoursWorked { get; init; }

    public required decimal TotalEarnings { get; init; }
}
