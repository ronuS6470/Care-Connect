using System.Data;
using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Reporting;
using CareConnect.DTOs.Reports;
using CareConnect.Infrastructure.Data;
using CareConnect.Infrastructure.Errors;
using CareConnect.Queries.Security;
using Dapper;

namespace CareConnect.Queries.Reports.Repositories;

/// <summary>
/// Admin-only operational reports, one Dapper query per report (no N+1: every report is exactly
/// one round trip, with any "name" lookups done via JOIN rather than a per-row follow-up query).
///
/// Hours/earnings figures use the same "actual check-in/check-out, decimal-only" rule as
/// CaregiverEarningsCalculator — DATEDIFF(SECOND, ...) cast to DECIMAL before dividing by 3600, so
/// the aggregation never touches float/double, done here in SQL (not C#) since these are
/// aggregates over many rows, not a single caregiver's payroll figure.
///
/// Performance notes (see also the class-level remarks in DashboardDapperRepository): most of
/// these queries filter/sort on ActualStartUtc or ScheduledStartUtc. The schema currently has an
/// index on (CaregiverAssignmentId, ScheduledStartUtc) and one on Status, but none on
/// ActualStartUtc — for a large Visits table, the hours/earnings/duration reports (which all
/// filter on ActualStartUtc) would benefit from `CREATE INDEX IX_Visits_ActualStartUtc ON
/// Visits(ActualStartUtc) WHERE ActualStartUtc IS NOT NULL` (a filtered index, since most visits
/// eventually have it set but historical volume grows). Not added here since it's a schema change
/// outside this task's scope — flagged for a future migration once visit volume warrants it.
/// </summary>
public sealed class ReportsDapperRepository : IReportsReadRepository
{
    private const string VisitSummaryColumns = """
        v.Id AS VisitId, cgu.FirstName + ' ' + cgu.LastName AS CaregiverFullName,
        clu.FirstName + ' ' + clu.LastName AS ClientFullName,
        v.ScheduledStartUtc, v.ScheduledEndUtc, v.Status
        """;

    private const string VisitJoins = """
        FROM Visits v
        INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
        INNER JOIN Caregivers cg ON cg.Id = a.CaregiverId
        INNER JOIN Users cgu ON cgu.Id = cg.UserId
        INNER JOIN Clients cl ON cl.Id = a.ClientId
        INNER JOIN Users clu ON clu.Id = cl.UserId
        """;

    private const string WorkedHoursExpression = "CAST(DATEDIFF(SECOND, v.ActualStartUtc, v.ActualEndUtc) AS DECIMAL(18,4)) / 3600.0";

    private readonly IDbConnectionFactory _connectionFactory;

    public ReportsDapperRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<VisitSummaryDto>> GetTodaysVisitsAsync(string requestingAuth0UserId, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await EnsureAdminAsync(connection, requestingAuth0UserId, cancellationToken);

        var sql = $"""
            SELECT {VisitSummaryColumns}
            {VisitJoins}
            WHERE CAST(v.ScheduledStartUtc AS date) = @Today
            ORDER BY v.ScheduledStartUtc;
            """;

        var visits = await connection.QueryAsync<VisitSummaryDto>(new CommandDefinition(
            sql, new { Today = DateOnly.FromDateTime(DateTime.UtcNow) }, cancellationToken: cancellationToken));

        return visits.ToList();
    }

    public async Task<IReadOnlyList<VisitSummaryDto>> GetCompletedVisitsAsync(
        DateOnly fromDate, DateOnly toDate, string requestingAuth0UserId, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await EnsureAdminAsync(connection, requestingAuth0UserId, cancellationToken);

        const string sql = $"""
            SELECT {VisitSummaryColumns}
            {VisitJoins}
            WHERE v.Status = @Completed
              AND CAST(v.ScheduledStartUtc AS date) BETWEEN @FromDate AND @ToDate
            ORDER BY v.ScheduledStartUtc;
            """;

        var visits = await connection.QueryAsync<VisitSummaryDto>(new CommandDefinition(
            sql,
            new { FromDate = fromDate, ToDate = toDate, Completed = VisitStatus.Completed },
            cancellationToken: cancellationToken));

        return visits.ToList();
    }

    public async Task<IReadOnlyList<VisitsPerCaregiverReportRowDto>> GetVisitsPerCaregiverAsync(
        DateOnly fromDate, DateOnly toDate, string requestingAuth0UserId, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await EnsureAdminAsync(connection, requestingAuth0UserId, cancellationToken);

        const string sql = """
            SELECT cg.Id AS CaregiverId, cgu.FirstName + ' ' + cgu.LastName AS CaregiverFullName,
                   COUNT(*) AS TotalVisits,
                   SUM(CASE WHEN v.Status = @Completed THEN 1 ELSE 0 END) AS CompletedVisits,
                   SUM(CASE WHEN v.Status = @Cancelled THEN 1 ELSE 0 END) AS CancelledVisits,
                   SUM(CASE WHEN v.Status = @NoShow THEN 1 ELSE 0 END) AS NoShowVisits
            FROM Visits v
            INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
            INNER JOIN Caregivers cg ON cg.Id = a.CaregiverId
            INNER JOIN Users cgu ON cgu.Id = cg.UserId
            WHERE CAST(v.ScheduledStartUtc AS date) BETWEEN @FromDate AND @ToDate
            GROUP BY cg.Id, cgu.FirstName, cgu.LastName
            ORDER BY TotalVisits DESC;
            """;

        var rows = await connection.QueryAsync<VisitsPerCaregiverReportRowDto>(new CommandDefinition(
            sql,
            new
            {
                FromDate = fromDate,
                ToDate = toDate,
                Completed = VisitStatus.Completed,
                Cancelled = VisitStatus.Cancelled,
                NoShow = VisitStatus.NoShow,
            },
            cancellationToken: cancellationToken));

        return rows.ToList();
    }

    public async Task<IReadOnlyList<VisitsPerClientReportRowDto>> GetVisitsPerClientAsync(
        DateOnly fromDate, DateOnly toDate, string requestingAuth0UserId, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await EnsureAdminAsync(connection, requestingAuth0UserId, cancellationToken);

        const string sql = """
            SELECT cl.Id AS ClientId, clu.FirstName + ' ' + clu.LastName AS ClientFullName,
                   COUNT(*) AS TotalVisits,
                   SUM(CASE WHEN v.Status = @Completed THEN 1 ELSE 0 END) AS CompletedVisits,
                   SUM(CASE WHEN v.Status = @Cancelled THEN 1 ELSE 0 END) AS CancelledVisits,
                   SUM(CASE WHEN v.Status = @NoShow THEN 1 ELSE 0 END) AS NoShowVisits
            FROM Visits v
            INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
            INNER JOIN Clients cl ON cl.Id = a.ClientId
            INNER JOIN Users clu ON clu.Id = cl.UserId
            WHERE CAST(v.ScheduledStartUtc AS date) BETWEEN @FromDate AND @ToDate
            GROUP BY cl.Id, clu.FirstName, clu.LastName
            ORDER BY TotalVisits DESC;
            """;

        var rows = await connection.QueryAsync<VisitsPerClientReportRowDto>(new CommandDefinition(
            sql,
            new
            {
                FromDate = fromDate,
                ToDate = toDate,
                Completed = VisitStatus.Completed,
                Cancelled = VisitStatus.Cancelled,
                NoShow = VisitStatus.NoShow,
            },
            cancellationToken: cancellationToken));

        return rows.ToList();
    }

    public async Task<IReadOnlyList<CaregiverHoursReportRowDto>> GetCaregiverTotalHoursAsync(
        DateOnly fromDate, DateOnly toDate, string requestingAuth0UserId, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await EnsureAdminAsync(connection, requestingAuth0UserId, cancellationToken);

        const string sql = $"""
            SELECT cg.Id AS CaregiverId, cgu.FirstName + ' ' + cgu.LastName AS CaregiverFullName,
                   COUNT(*) AS CompletedVisitCount,
                   SUM({WorkedHoursExpression}) AS TotalHoursWorked
            FROM Visits v
            INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
            INNER JOIN Caregivers cg ON cg.Id = a.CaregiverId
            INNER JOIN Users cgu ON cgu.Id = cg.UserId
            WHERE v.Status = @Completed AND v.ActualStartUtc IS NOT NULL AND v.ActualEndUtc IS NOT NULL
              AND CAST(v.ActualStartUtc AS date) BETWEEN @FromDate AND @ToDate
            GROUP BY cg.Id, cgu.FirstName, cgu.LastName
            ORDER BY TotalHoursWorked DESC;
            """;

        var rows = await connection.QueryAsync<CaregiverHoursReportRowDto>(new CommandDefinition(
            sql,
            new { FromDate = fromDate, ToDate = toDate, Completed = VisitStatus.Completed },
            cancellationToken: cancellationToken));

        return rows.ToList();
    }

    public async Task<IReadOnlyList<CaregiverEarningsReportRowDto>> GetCaregiverTotalEarningsAsync(
        DateOnly fromDate, DateOnly toDate, string requestingAuth0UserId, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await EnsureAdminAsync(connection, requestingAuth0UserId, cancellationToken);

        const string sql = $"""
            SELECT cg.Id AS CaregiverId, cgu.FirstName + ' ' + cgu.LastName AS CaregiverFullName,
                   cg.HourlyRate,
                   SUM({WorkedHoursExpression}) AS TotalHoursWorked,
                   SUM({WorkedHoursExpression} * cg.HourlyRate) AS TotalEarnings
            FROM Visits v
            INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
            INNER JOIN Caregivers cg ON cg.Id = a.CaregiverId
            INNER JOIN Users cgu ON cgu.Id = cg.UserId
            WHERE v.Status = @Completed AND v.ActualStartUtc IS NOT NULL AND v.ActualEndUtc IS NOT NULL
              AND CAST(v.ActualStartUtc AS date) BETWEEN @FromDate AND @ToDate
            GROUP BY cg.Id, cgu.FirstName, cgu.LastName, cg.HourlyRate
            ORDER BY TotalEarnings DESC;
            """;

        var rows = await connection.QueryAsync<CaregiverEarningsReportRowDto>(new CommandDefinition(
            sql,
            new { FromDate = fromDate, ToDate = toDate, Completed = VisitStatus.Completed },
            cancellationToken: cancellationToken));

        return rows.ToList();
    }

    public async Task<IReadOnlyList<ClientWithoutActiveCaregiverDto>> GetClientsWithoutActiveCaregiverAsync(
        string requestingAuth0UserId, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await EnsureAdminAsync(connection, requestingAuth0UserId, cancellationToken);

        const string sql = """
            SELECT c.Id AS ClientId, u.FirstName + ' ' + u.LastName AS ClientFullName, c.IsActive
            FROM Clients c
            INNER JOIN Users u ON u.Id = c.UserId
            LEFT JOIN CaregiverAssignments a ON a.ClientId = c.Id AND a.Status = @ActiveAssignment
            WHERE a.Id IS NULL
            ORDER BY c.IsActive DESC, ClientFullName;
            """;

        var rows = await connection.QueryAsync<ClientWithoutActiveCaregiverDto>(new CommandDefinition(
            sql, new { ActiveAssignment = AssignmentStatus.Active }, cancellationToken: cancellationToken));

        return rows.ToList();
    }

    public async Task<IReadOnlyList<CaregiverWithoutVisitsTodayDto>> GetCaregiversWithoutVisitsTodayAsync(
        string requestingAuth0UserId, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await EnsureAdminAsync(connection, requestingAuth0UserId, cancellationToken);

        var sql = """
            SELECT c.Id AS CaregiverId, u.FirstName + ' ' + u.LastName AS CaregiverFullName
            FROM Caregivers c
            INNER JOIN Users u ON u.Id = c.UserId
            WHERE c.IsActive = 1
              AND NOT EXISTS (
                  SELECT 1
                  FROM Visits v
                  INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
                  WHERE a.CaregiverId = c.Id AND CAST(v.ScheduledStartUtc AS date) = @Today
              )
            ORDER BY CaregiverFullName;
            """;

        var rows = await connection.QueryAsync<CaregiverWithoutVisitsTodayDto>(new CommandDefinition(
            sql, new { Today = DateOnly.FromDateTime(DateTime.UtcNow) }, cancellationToken: cancellationToken));

        return rows.ToList();
    }

    public async Task<IReadOnlyList<HighestEarningCaregiverReportRowDto>> GetHighestEarningCaregiversAsync(
        DateOnly fromDate, DateOnly toDate, int topN, string requestingAuth0UserId, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await EnsureAdminAsync(connection, requestingAuth0UserId, cancellationToken);

        const string sql = $"""
            WITH CaregiverEarnings AS (
                SELECT cg.Id AS CaregiverId, cgu.FirstName + ' ' + cgu.LastName AS CaregiverFullName,
                       SUM({WorkedHoursExpression} * cg.HourlyRate) AS TotalEarnings
                FROM Visits v
                INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
                INNER JOIN Caregivers cg ON cg.Id = a.CaregiverId
                INNER JOIN Users cgu ON cgu.Id = cg.UserId
                WHERE v.Status = @Completed AND v.ActualStartUtc IS NOT NULL AND v.ActualEndUtc IS NOT NULL
                  AND CAST(v.ActualStartUtc AS date) BETWEEN @FromDate AND @ToDate
                GROUP BY cg.Id, cgu.FirstName, cgu.LastName
            )
            SELECT TOP (@TopN)
                CAST(RANK() OVER (ORDER BY TotalEarnings DESC) AS INT) AS Rank,
                CaregiverId, CaregiverFullName, TotalEarnings
            FROM CaregiverEarnings
            ORDER BY TotalEarnings DESC;
            """;

        var rows = await connection.QueryAsync<HighestEarningCaregiverReportRowDto>(new CommandDefinition(
            sql,
            new { FromDate = fromDate, ToDate = toDate, TopN = topN, Completed = VisitStatus.Completed },
            cancellationToken: cancellationToken));

        return rows.ToList();
    }

    public async Task<IReadOnlyList<CareTaskDemandReportRowDto>> GetMostRequestedCareTasksAsync(
        DateOnly fromDate, DateOnly toDate, string requestingAuth0UserId, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await EnsureAdminAsync(connection, requestingAuth0UserId, cancellationToken);

        var sql = """
            SELECT ct.Id AS CareTaskId, ct.Name AS CareTaskName,
                   COUNT(*) AS TimesRequested,
                   SUM(CASE WHEN vt.IsCompleted = 1 THEN 1 ELSE 0 END) AS TimesCompleted
            FROM VisitTasks vt
            INNER JOIN CareTasks ct ON ct.Id = vt.CareTaskId
            INNER JOIN Visits v ON v.Id = vt.VisitId
            WHERE CAST(v.ScheduledStartUtc AS date) BETWEEN @FromDate AND @ToDate
            GROUP BY ct.Id, ct.Name
            ORDER BY TimesRequested DESC;
            """;

        var rows = await connection.QueryAsync<CareTaskDemandReportRowDto>(new CommandDefinition(
            sql, new { FromDate = fromDate, ToDate = toDate }, cancellationToken: cancellationToken));

        return rows.ToList();
    }

    public async Task<IReadOnlyList<AverageVisitDurationReportRowDto>> GetAverageVisitDurationAsync(
        DateOnly fromDate, DateOnly toDate, string requestingAuth0UserId, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await EnsureAdminAsync(connection, requestingAuth0UserId, cancellationToken);

        const string sql = $"""
            SELECT cg.Id AS CaregiverId, cgu.FirstName + ' ' + cgu.LastName AS CaregiverFullName,
                   COUNT(*) AS CompletedVisitCount,
                   AVG({WorkedHoursExpression}) AS AverageVisitDurationHours
            FROM Visits v
            INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
            INNER JOIN Caregivers cg ON cg.Id = a.CaregiverId
            INNER JOIN Users cgu ON cgu.Id = cg.UserId
            WHERE v.Status = @Completed AND v.ActualStartUtc IS NOT NULL AND v.ActualEndUtc IS NOT NULL
              AND CAST(v.ActualStartUtc AS date) BETWEEN @FromDate AND @ToDate
            GROUP BY cg.Id, cgu.FirstName, cgu.LastName
            ORDER BY AverageVisitDurationHours DESC;
            """;

        var rows = await connection.QueryAsync<AverageVisitDurationReportRowDto>(new CommandDefinition(
            sql,
            new { FromDate = fromDate, ToDate = toDate, Completed = VisitStatus.Completed },
            cancellationToken: cancellationToken));

        return rows.ToList();
    }

    public async Task<IReadOnlyList<TopCompletedVisitsCaregiverReportRowDto>> GetTopCaregiversByCompletedVisitsAsync(
        DateOnly fromDate, DateOnly toDate, int topN, string requestingAuth0UserId, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await EnsureAdminAsync(connection, requestingAuth0UserId, cancellationToken);

        const string sql = """
            WITH CaregiverCompletedCounts AS (
                SELECT cg.Id AS CaregiverId, cgu.FirstName + ' ' + cgu.LastName AS CaregiverFullName,
                       COUNT(*) AS CompletedVisitCount
                FROM Visits v
                INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
                INNER JOIN Caregivers cg ON cg.Id = a.CaregiverId
                INNER JOIN Users cgu ON cgu.Id = cg.UserId
                WHERE v.Status = @Completed AND v.ActualStartUtc IS NOT NULL AND v.ActualEndUtc IS NOT NULL
                  AND CAST(v.ActualStartUtc AS date) BETWEEN @FromDate AND @ToDate
                GROUP BY cg.Id, cgu.FirstName, cgu.LastName
            )
            SELECT TOP (@TopN)
                CAST(ROW_NUMBER() OVER (ORDER BY CompletedVisitCount DESC) AS INT) AS Rank,
                CaregiverId, CaregiverFullName, CompletedVisitCount
            FROM CaregiverCompletedCounts
            ORDER BY CompletedVisitCount DESC;
            """;

        var rows = await connection.QueryAsync<TopCompletedVisitsCaregiverReportRowDto>(new CommandDefinition(
            sql,
            new { FromDate = fromDate, ToDate = toDate, TopN = topN, Completed = VisitStatus.Completed },
            cancellationToken: cancellationToken));

        return rows.ToList();
    }

    public async Task<IReadOnlyList<CaregiverAvailabilityConflictDto>> GetCaregiverAvailabilityConflictsAsync(
        string requestingAuth0UserId, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await EnsureAdminAsync(connection, requestingAuth0UserId, cancellationToken);

        // Day-of-week deliberately isn't computed in SQL: DATEPART(WEEKDAY, ...) depends on the
        // session's SET DATEFIRST setting, which isn't guaranteed and would silently misalign
        // CaregiverAvailability.DayOfWeek (stored using .NET's Sunday=0..Saturday=6) with whatever
        // the server's locale happens to be. Fetching raw rows and comparing DayOfWeek in C#
        // (DateTime.DayOfWeek) sidesteps that ambiguity entirely.
        const string sql = """
            WITH ActiveVisits AS (
                SELECT v.Id AS VisitId, a.CaregiverId, cgu.FirstName + ' ' + cgu.LastName AS CaregiverFullName,
                       v.ScheduledStartUtc, v.ScheduledEndUtc
                FROM Visits v
                INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
                INNER JOIN Caregivers cg ON cg.Id = a.CaregiverId
                INNER JOIN Users cgu ON cgu.Id = cg.UserId
                WHERE v.Status IN (@Scheduled, @InProgress)
            )
            SELECT VisitId, CaregiverId, CaregiverFullName, ScheduledStartUtc, ScheduledEndUtc
            FROM ActiveVisits
            ORDER BY ScheduledStartUtc;

            WITH ActiveVisits AS (
                SELECT v.Id AS VisitId, a.CaregiverId, cgu.FirstName + ' ' + cgu.LastName AS CaregiverFullName,
                       v.ScheduledStartUtc, v.ScheduledEndUtc
                FROM Visits v
                INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
                INNER JOIN Caregivers cg ON cg.Id = a.CaregiverId
                INNER JOIN Users cgu ON cgu.Id = cg.UserId
                WHERE v.Status IN (@Scheduled, @InProgress)
            )
            SELECT av1.CaregiverId, av1.CaregiverFullName, av1.VisitId, av1.ScheduledStartUtc, av1.ScheduledEndUtc,
                   av2.VisitId AS ConflictingVisitId
            FROM ActiveVisits av1
            INNER JOIN ActiveVisits av2
                ON av2.CaregiverId = av1.CaregiverId
               AND av2.VisitId <> av1.VisitId
               AND av1.ScheduledStartUtc < av2.ScheduledEndUtc
               AND av1.ScheduledEndUtc > av2.ScheduledStartUtc
               AND av1.VisitId < av2.VisitId;

            SELECT CaregiverId, DayOfWeek, StartTime, EndTime
            FROM CaregiverAvailabilities
            WHERE IsActive = 1;
            """;

        using var multi = await connection.QueryMultipleAsync(new CommandDefinition(
            sql,
            new { Scheduled = VisitStatus.Scheduled, InProgress = VisitStatus.InProgress },
            cancellationToken: cancellationToken));

        var activeVisits = (await multi.ReadAsync<ActiveVisitRow>()).ToList();
        var overlaps = (await multi.ReadAsync<OverlapRow>()).ToList();
        var availabilityByCaregiver = (await multi.ReadAsync<AvailabilityWindowRow>()).ToLookup(w => w.CaregiverId);

        var conflicts = new List<CaregiverAvailabilityConflictDto>();

        foreach (var visit in activeVisits)
        {
            var dayOfWeek = visit.ScheduledStartUtc.DayOfWeek;
            var startTime = TimeOnly.FromDateTime(visit.ScheduledStartUtc);
            var endTime = TimeOnly.FromDateTime(visit.ScheduledEndUtc);

            var coveredByAvailability = availabilityByCaregiver[visit.CaregiverId].Any(w =>
                w.DayOfWeek == dayOfWeek && w.StartTime <= startTime && w.EndTime >= endTime);

            if (!coveredByAvailability)
            {
                conflicts.Add(new CaregiverAvailabilityConflictDto
                {
                    ConflictType = "OutsideAvailability",
                    CaregiverId = visit.CaregiverId,
                    CaregiverFullName = visit.CaregiverFullName,
                    VisitId = visit.VisitId,
                    ScheduledStartUtc = visit.ScheduledStartUtc,
                    ScheduledEndUtc = visit.ScheduledEndUtc,
                    ConflictingVisitId = null,
                    Details = "No active availability window covers this visit's day/time.",
                });
            }
        }

        foreach (var overlap in overlaps)
        {
            conflicts.Add(new CaregiverAvailabilityConflictDto
            {
                ConflictType = "OverlappingVisits",
                CaregiverId = overlap.CaregiverId,
                CaregiverFullName = overlap.CaregiverFullName,
                VisitId = overlap.VisitId,
                ScheduledStartUtc = overlap.ScheduledStartUtc,
                ScheduledEndUtc = overlap.ScheduledEndUtc,
                ConflictingVisitId = overlap.ConflictingVisitId,
                Details = $"Overlaps with visit #{overlap.ConflictingVisitId} for the same caregiver.",
            });
        }

        return conflicts;
    }

    public async Task<IReadOnlyList<TopCaregiverByHoursReportRowDto>> GetTopCaregiversByHoursAsync(
        DateOnly fromDate, DateOnly toDate, int topN, string requestingAuth0UserId, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await EnsureAdminAsync(connection, requestingAuth0UserId, cancellationToken);

        // TOP (N) + ORDER BY here instead of a window function — genuinely simpler for a plain
        // "give me the top N" cut, no need for RANK()/ROW_NUMBER()'s tie-handling semantics.
        const string sql = $"""
            SELECT TOP (@TopN) cg.Id AS CaregiverId, cgu.FirstName + ' ' + cgu.LastName AS CaregiverFullName,
                   SUM({WorkedHoursExpression}) AS TotalHoursWorked
            FROM Visits v
            INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
            INNER JOIN Caregivers cg ON cg.Id = a.CaregiverId
            INNER JOIN Users cgu ON cgu.Id = cg.UserId
            WHERE v.Status = @Completed AND v.ActualStartUtc IS NOT NULL AND v.ActualEndUtc IS NOT NULL
              AND CAST(v.ActualStartUtc AS date) BETWEEN @FromDate AND @ToDate
            GROUP BY cg.Id, cgu.FirstName, cgu.LastName
            ORDER BY TotalHoursWorked DESC;
            """;

        var rows = (await connection.QueryAsync<TopCaregiverByHoursRow>(new CommandDefinition(
            sql,
            new { FromDate = fromDate, ToDate = toDate, TopN = topN, Completed = VisitStatus.Completed },
            cancellationToken: cancellationToken))).ToList();

        return rows
            .Select((row, index) => new TopCaregiverByHoursReportRowDto
            {
                Rank = index + 1,
                CaregiverId = row.CaregiverId,
                CaregiverFullName = row.CaregiverFullName,
                TotalHoursWorked = row.TotalHoursWorked,
            })
            .ToList();
    }

    private static async Task EnsureAdminAsync(IDbConnection connection, string requestingAuth0UserId, CancellationToken cancellationToken)
    {
        var requester = await RequesterResolver.ResolveAsync(connection, requestingAuth0UserId, cancellationToken);

        if (requester.Role != UserRole.Admin)
        {
            throw new ForbiddenException("Only Admin may view this report.");
        }
    }

    private sealed class ActiveVisitRow
    {
        public int VisitId { get; init; }

        public int CaregiverId { get; init; }

        public string CaregiverFullName { get; init; } = string.Empty;

        public DateTime ScheduledStartUtc { get; init; }

        public DateTime ScheduledEndUtc { get; init; }
    }

    private sealed class OverlapRow
    {
        public int CaregiverId { get; init; }

        public string CaregiverFullName { get; init; } = string.Empty;

        public int VisitId { get; init; }

        public DateTime ScheduledStartUtc { get; init; }

        public DateTime ScheduledEndUtc { get; init; }

        public int ConflictingVisitId { get; init; }
    }

    private sealed class AvailabilityWindowRow
    {
        public int CaregiverId { get; init; }

        public DayOfWeek DayOfWeek { get; init; }

        public TimeOnly StartTime { get; init; }

        public TimeOnly EndTime { get; init; }
    }

    private sealed class TopCaregiverByHoursRow
    {
        public int CaregiverId { get; init; }

        public string CaregiverFullName { get; init; } = string.Empty;

        public decimal TotalHoursWorked { get; init; }
    }
}
