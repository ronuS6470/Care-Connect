using CareConnect.DTOs.Reporting;
using CareConnect.Infrastructure.Data;
using CareConnect.Infrastructure.Errors;
using CareConnect.Queries.Security;
using Dapper;

namespace CareConnect.Queries.Reporting.Repositories;

public sealed class CaregiverEarningsDapperRepository : ICaregiverEarningsReadRepository
{
    private const string CaregiverSql = """
        SELECT c.Id, c.HourlyRate, u.FirstName + ' ' + u.LastName AS FullName
        FROM Caregivers c
        INNER JOIN Users u ON u.Id = c.UserId
        WHERE c.Id = @CaregiverId;
        """;

    // Filtered by ActualStartUtc, not ScheduledStartUtc — "date range" means the period actually
    // worked, matching the rule that earnings/hours are computed from actual check-in/check-out.
    // A visit with no ActualStartUtc yet (Scheduled) can never match; one that was checked in and
    // then cancelled before checkout is still fetched (it has an ActualStartUtc) but contributes
    // nothing — CaregiverEarningsCalculator.FilterCompleted is what actually decides that.
    private const string VisitsSql = """
        SELECT v.Id AS VisitId, clu.FirstName + ' ' + clu.LastName AS ClientFullName,
               v.Status, v.ActualStartUtc, v.ActualEndUtc
        FROM Visits v
        INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
        INNER JOIN Clients cl ON cl.Id = a.ClientId
        INNER JOIN Users clu ON clu.Id = cl.UserId
        WHERE a.CaregiverId = @CaregiverId
          AND v.ActualStartUtc IS NOT NULL
          AND CAST(v.ActualStartUtc AS date) BETWEEN @FromDate AND @ToDate
        ORDER BY v.ActualStartUtc;
        """;

    private readonly IDbConnectionFactory _connectionFactory;

    public CaregiverEarningsDapperRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<CaregiverEarningsDto> GetEarningsAsync(
        int caregiverId,
        DateOnly fromDate,
        DateOnly toDate,
        bool includeVisitBreakdown,
        string requestingAuth0UserId,
        CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var requester = await RequesterResolver.ResolveAsync(connection, requestingAuth0UserId, cancellationToken);

        if (!CaregiverEarningsAuthorizer.CanView(requester, caregiverId))
        {
            throw new ForbiddenException("You do not have access to this caregiver's earnings.");
        }

        var caregiver = await connection.QuerySingleOrDefaultAsync<CaregiverRow>(new CommandDefinition(
            CaregiverSql, new { CaregiverId = caregiverId }, cancellationToken: cancellationToken))
            ?? throw new NotFoundException($"Caregiver {caregiverId} was not found.");

        var visits = await connection.QueryAsync<CaregiverVisitRecord>(new CommandDefinition(
            VisitsSql,
            new { CaregiverId = caregiverId, FromDate = fromDate, ToDate = toDate },
            cancellationToken: cancellationToken));

        var (totalHours, totalEarnings, completedCount, lines) = CaregiverEarningsCalculator.Calculate(visits, caregiver.HourlyRate);

        return new CaregiverEarningsDto
        {
            CaregiverId = caregiverId,
            CaregiverFullName = caregiver.FullName,
            PeriodStart = fromDate,
            PeriodEnd = toDate,
            HourlyRate = caregiver.HourlyRate,
            TotalHoursWorked = totalHours,
            TotalEarnings = totalEarnings,
            CompletedVisitCount = completedCount,
            Visits = includeVisitBreakdown ? lines : null,
        };
    }

    public async Task<IReadOnlyList<WorkedHoursDto>> GetHoursByDateAsync(
        int caregiverId,
        DateOnly fromDate,
        DateOnly toDate,
        string requestingAuth0UserId,
        CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var requester = await RequesterResolver.ResolveAsync(connection, requestingAuth0UserId, cancellationToken);

        if (!CaregiverEarningsAuthorizer.CanView(requester, caregiverId))
        {
            throw new ForbiddenException("You do not have access to this caregiver's hours.");
        }

        var caregiver = await connection.QuerySingleOrDefaultAsync<CaregiverRow>(new CommandDefinition(
            CaregiverSql, new { CaregiverId = caregiverId }, cancellationToken: cancellationToken))
            ?? throw new NotFoundException($"Caregiver {caregiverId} was not found.");

        var visits = await connection.QueryAsync<CaregiverVisitRecord>(new CommandDefinition(
            VisitsSql,
            new { CaregiverId = caregiverId, FromDate = fromDate, ToDate = toDate },
            cancellationToken: cancellationToken));

        return CaregiverEarningsCalculator.FilterCompleted(visits)
            .GroupBy(x => DateOnly.FromDateTime(x.CheckInUtc))
            .OrderBy(g => g.Key)
            .Select(g => new WorkedHoursDto
            {
                CaregiverId = caregiverId,
                CaregiverFullName = caregiver.FullName,
                Date = g.Key,
                HoursWorked = CaregiverEarningsCalculator.RoundHours(
                    g.Sum(x => CaregiverEarningsCalculator.CalculateWorkedHours(x.CheckInUtc, x.CheckOutUtc))),
            })
            .ToList();
    }

    private sealed class CaregiverRow
    {
        public int Id { get; init; }

        public decimal HourlyRate { get; init; }

        public string FullName { get; init; } = string.Empty;
    }
}
