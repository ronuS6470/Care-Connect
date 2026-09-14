namespace CareConnect.DTOs.Reporting;

public sealed class WorkedHoursDto
{
    public required int CaregiverId { get; init; }

    public required string CaregiverFullName { get; init; }

    public required DateOnly Date { get; init; }

    public required decimal HoursWorked { get; init; }
}
