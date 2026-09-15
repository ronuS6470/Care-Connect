using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Reporting;

namespace CareConnect.Queries.Reporting;

/// <summary>
/// A visit as far as earnings/hours calculation is concerned — raw actual check-in/check-out
/// times, never the scheduled window. Anything that isn't Completed with both actual timestamps
/// recorded contributes zero hours and zero earnings.
/// </summary>
public sealed class CaregiverVisitRecord
{
    public int VisitId { get; init; }

    public string ClientFullName { get; init; } = string.Empty;

    public VisitStatus Status { get; init; }

    public DateTime? ActualStartUtc { get; init; }

    public DateTime? ActualEndUtc { get; init; }
}

/// <summary>
/// Pure earnings/hours math for completed visits — no I/O, so every rule here is independently
/// unit-testable without a database.
///
/// Rounding policy: per-visit and aggregate figures are summed at full decimal precision (never
/// rounded mid-calculation, so rounding error can't compound across many visits); rounding to 2
/// decimal places happens exactly once, at the point a number is about to be returned — the
/// per-visit breakdown lines and the aggregate totals are each rounded independently from the
/// same unrounded running sums. Rounding uses <see cref="MidpointRounding.AwayFromZero"/> (the
/// conventional "round half up" for money) rather than .NET's default banker's rounding, so e.g.
/// 2.005 rounds to 2.01, not 2.00.
/// </summary>
public static class CaregiverEarningsCalculator
{
    private const int DecimalPlaces = 2;

    /// <summary>WorkedHours = CheckOutTime - CheckInTime, expressed in hours.</summary>
    public static decimal CalculateWorkedHours(DateTime checkInUtc, DateTime checkOutUtc)
    {
        if (checkOutUtc <= checkInUtc)
        {
            throw new ArgumentOutOfRangeException(nameof(checkOutUtc), "Check-out must be after check-in.");
        }

        // (checkOutUtc - checkInUtc).Ticks is a long; casting to decimal before dividing keeps
        // this exact. Never route through double/TimeSpan.TotalHours — that would reintroduce the
        // floating-point error this calculator exists to avoid.
        return (decimal)(checkOutUtc - checkInUtc).Ticks / TimeSpan.TicksPerHour;
    }

    /// <summary>Earnings = WorkedHours × Caregiver.HourlyRate.</summary>
    public static decimal CalculateEarnings(decimal workedHours, decimal hourlyRate) => workedHours * hourlyRate;

    public static decimal RoundHours(decimal hours) => Math.Round(hours, DecimalPlaces, MidpointRounding.AwayFromZero);

    public static decimal RoundMoney(decimal amount) => Math.Round(amount, DecimalPlaces, MidpointRounding.AwayFromZero);

    /// <summary>
    /// The single place that decides which visits count: Completed status AND both actual
    /// timestamps present. Cancelled, Scheduled, InProgress, and NoShow visits are excluded —
    /// including a visit that was checked in and then cancelled before checkout (it has an
    /// ActualStartUtc but no ActualEndUtc, and/or a non-Completed status either way).
    /// </summary>
    public static IEnumerable<(CaregiverVisitRecord Visit, DateTime CheckInUtc, DateTime CheckOutUtc)> FilterCompleted(
        IEnumerable<CaregiverVisitRecord> visits)
    {
        foreach (var visit in visits)
        {
            if (visit.Status == VisitStatus.Completed && visit.ActualStartUtc is { } checkIn && visit.ActualEndUtc is { } checkOut)
            {
                yield return (visit, checkIn, checkOut);
            }
        }
    }

    /// <summary>Computes totals and a per-visit breakdown across every completed visit in the input.</summary>
    public static (decimal TotalHoursWorked, decimal TotalEarnings, int CompletedVisitCount, IReadOnlyList<CaregiverEarningsVisitDto> Visits)
        Calculate(IEnumerable<CaregiverVisitRecord> visits, decimal hourlyRate)
    {
        var lines = new List<CaregiverEarningsVisitDto>();
        var totalHoursUnrounded = 0m;
        var totalEarningsUnrounded = 0m;

        foreach (var (visit, checkIn, checkOut) in FilterCompleted(visits))
        {
            var hours = CalculateWorkedHours(checkIn, checkOut);
            var earnings = CalculateEarnings(hours, hourlyRate);

            totalHoursUnrounded += hours;
            totalEarningsUnrounded += earnings;

            lines.Add(new CaregiverEarningsVisitDto
            {
                VisitId = visit.VisitId,
                ClientFullName = visit.ClientFullName,
                CheckInUtc = checkIn,
                CheckOutUtc = checkOut,
                WorkedHours = RoundHours(hours),
                Earnings = RoundMoney(earnings),
            });
        }

        return (RoundHours(totalHoursUnrounded), RoundMoney(totalEarningsUnrounded), lines.Count, lines);
    }
}
