namespace CareConnect.DTOs.Reporting;

public sealed class CaregiverEarningsDto
{
    public required int CaregiverId { get; init; }

    public required string CaregiverFullName { get; init; }

    public required DateOnly PeriodStart { get; init; }

    public required DateOnly PeriodEnd { get; init; }

    public required decimal HourlyRate { get; init; }

    public required decimal TotalHoursWorked { get; init; }

    public required decimal TotalEarnings { get; init; }

    public required int CompletedVisitCount { get; init; }

    /// <summary>Per-visit breakdown — only populated when the caller asked for it; null otherwise.</summary>
    public IReadOnlyList<CaregiverEarningsVisitDto>? Visits { get; init; }
}
