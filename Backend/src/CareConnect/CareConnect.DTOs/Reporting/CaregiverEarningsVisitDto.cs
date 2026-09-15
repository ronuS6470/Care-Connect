namespace CareConnect.DTOs.Reporting;

/// <summary>One visit's contribution to a caregiver earnings report — actual check-in/check-out only.</summary>
public sealed class CaregiverEarningsVisitDto
{
    public required int VisitId { get; init; }

    public required string ClientFullName { get; init; }

    public required DateTime CheckInUtc { get; init; }

    public required DateTime CheckOutUtc { get; init; }

    public required decimal WorkedHours { get; init; }

    public required decimal Earnings { get; init; }
}
