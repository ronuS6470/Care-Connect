using CareConnect.DTOs.Enums;
using CareConnect.Queries.Reporting;
using FluentAssertions;
using Xunit;

namespace CareConnect.Queries.Tests.Reporting;

public class CaregiverEarningsCalculatorTests
{
    private static readonly DateTime BaseCheckIn = new(2026, 3, 2, 9, 0, 0, DateTimeKind.Utc);

    // --- Scenario 1: 4 hours x $20 = $80 ---

    [Fact]
    public void CalculateWorkedHours_FourHourVisit_ReturnsExactlyFour()
    {
        var hours = CaregiverEarningsCalculator.CalculateWorkedHours(BaseCheckIn, BaseCheckIn.AddHours(4));

        hours.Should().Be(4.00m);
    }

    [Fact]
    public void CalculateEarnings_FourHoursAtTwentyPerHour_ReturnsEighty()
    {
        var earnings = CaregiverEarningsCalculator.CalculateEarnings(workedHours: 4.00m, hourlyRate: 20.00m);

        earnings.Should().Be(80.00m);
    }

    // --- Scenario 2: partial hours ---

    [Theory]
    [InlineData(0, 30, 20.00, 10.00)]    // 30 min * $20/hr
    [InlineData(1, 45, 22.00, 38.50)]    // 1h45m * $22/hr
    [InlineData(2, 15, 19.99, 44.9775)]  // 2h15m * $19.99/hr — exact in decimal, no rounding involved here
    public void CalculateWorkedHoursAndEarnings_PartialHours_AreExact(
        int hours, int minutes, decimal hourlyRate, decimal expectedEarnings)
    {
        var checkOut = BaseCheckIn.AddHours(hours).AddMinutes(minutes);

        var workedHours = CaregiverEarningsCalculator.CalculateWorkedHours(BaseCheckIn, checkOut);
        var earnings = CaregiverEarningsCalculator.CalculateEarnings(workedHours, hourlyRate);

        earnings.Should().Be(expectedEarnings);
    }

    // --- Scenario 3: multiple visits ---

    [Fact]
    public void Calculate_MultipleCompletedVisits_AggregatesHoursAndEarnings()
    {
        var visits = new[]
        {
            Completed(1, "Client A", BaseCheckIn, BaseCheckIn.AddHours(4)),                                          // 4h
            Completed(2, "Client B", BaseCheckIn.AddDays(1), BaseCheckIn.AddDays(1).AddHours(2).AddMinutes(30)),     // 2.5h
            Completed(3, "Client C", BaseCheckIn.AddDays(2), BaseCheckIn.AddDays(2).AddHours(1).AddMinutes(15)),     // 1.25h
        };

        var (totalHours, totalEarnings, count, lines) = CaregiverEarningsCalculator.Calculate(visits, hourlyRate: 20.00m);

        totalHours.Should().Be(7.75m);
        totalEarnings.Should().Be(155.00m);
        count.Should().Be(3);
        lines.Should().HaveCount(3);
        lines[0].WorkedHours.Should().Be(4.00m);
        lines[0].Earnings.Should().Be(80.00m);
    }

    // --- Scenario 4: cancelled visits excluded ---

    [Fact]
    public void Calculate_CancelledVisit_IsExcluded()
    {
        var visits = new[]
        {
            Completed(1, "Client A", BaseCheckIn, BaseCheckIn.AddHours(4)),
            new CaregiverVisitRecord
            {
                VisitId = 2,
                ClientFullName = "Client B",
                Status = VisitStatus.Cancelled,
                // Checked in, then cancelled before checkout — has a start time but no end time.
                ActualStartUtc = BaseCheckIn.AddDays(1),
                ActualEndUtc = null,
            },
        };

        var (totalHours, totalEarnings, count, lines) = CaregiverEarningsCalculator.Calculate(visits, hourlyRate: 20.00m);

        totalHours.Should().Be(4.00m);
        totalEarnings.Should().Be(80.00m);
        count.Should().Be(1);
        lines.Should().ContainSingle(l => l.VisitId == 1);
    }

    // --- Scenario 5: incomplete visits excluded ---

    [Theory]
    [MemberData(nameof(IncompleteVisitCases))]
    public void Calculate_IncompleteVisit_IsExcluded(VisitStatus status, DateTime? actualStartUtc, DateTime? actualEndUtc)
    {
        var visits = new[]
        {
            Completed(1, "Client A", BaseCheckIn, BaseCheckIn.AddHours(4)),
            new CaregiverVisitRecord
            {
                VisitId = 2,
                ClientFullName = "Client B",
                Status = status,
                ActualStartUtc = actualStartUtc,
                ActualEndUtc = actualEndUtc,
            },
        };

        var (totalHours, totalEarnings, count, lines) = CaregiverEarningsCalculator.Calculate(visits, hourlyRate: 20.00m);

        count.Should().Be(1);
        totalHours.Should().Be(4.00m);
        totalEarnings.Should().Be(80.00m);
        lines.Should().ContainSingle(l => l.VisitId == 1);
    }

    public static IEnumerable<object?[]> IncompleteVisitCases()
    {
        yield return [VisitStatus.Scheduled, null, null];
        yield return [VisitStatus.InProgress, BaseCheckIn.AddDays(1), null]; // checked in, not yet checked out
        yield return [VisitStatus.NoShow, null, null];
    }

    // --- Scenario 7: decimal precision ---

    [Fact]
    public void CalculateWorkedHours_SummedFractionalValues_AreExactDecimal()
    {
        // The classic 0.1 + 0.2 != 0.3 floating-point trap: 6 minutes = 0.1h, 12 minutes = 0.2h.
        // In IEEE-754 double this sum is 0.30000000000000004; in decimal it's exactly 0.3.
        var sixMinutes = CaregiverEarningsCalculator.CalculateWorkedHours(BaseCheckIn, BaseCheckIn.AddMinutes(6));
        var twelveMinutes = CaregiverEarningsCalculator.CalculateWorkedHours(BaseCheckIn, BaseCheckIn.AddMinutes(12));

        sixMinutes.Should().Be(0.1m);
        twelveMinutes.Should().Be(0.2m);
        (sixMinutes + twelveMinutes).Should().Be(0.3m);
    }

    [Fact]
    public void RoundMoney_UsesAwayFromZero_NotBankersRounding()
    {
        // 2.005 rounds to 2.00 under .NET's default banker's rounding (ToEven); this calculator's
        // documented policy is round-half-up, matching standard invoicing conventions.
        CaregiverEarningsCalculator.RoundMoney(2.005m).Should().Be(2.01m);
        CaregiverEarningsCalculator.RoundMoney(2.015m).Should().Be(2.02m);
    }

    [Fact]
    public void CalculateWorkedHours_CheckOutNotAfterCheckIn_Throws()
    {
        var act = () => CaregiverEarningsCalculator.CalculateWorkedHours(BaseCheckIn, BaseCheckIn);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    private static CaregiverVisitRecord Completed(int visitId, string clientFullName, DateTime checkIn, DateTime checkOut) =>
        new()
        {
            VisitId = visitId,
            ClientFullName = clientFullName,
            Status = VisitStatus.Completed,
            ActualStartUtc = checkIn,
            ActualEndUtc = checkOut,
        };
}
