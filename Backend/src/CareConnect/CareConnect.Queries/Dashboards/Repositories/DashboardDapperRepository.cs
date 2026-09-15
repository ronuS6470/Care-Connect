using CareConnect.DTOs.Dashboards;
using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Reporting;
using CareConnect.DTOs.Visits;
using CareConnect.Infrastructure.Data;
using CareConnect.Infrastructure.Errors;
using CareConnect.Queries.Security;
using Dapper;

namespace CareConnect.Queries.Dashboards.Repositories;

/// <summary>
/// Each dashboard is one connection, one round trip: Dapper's QueryMultipleAsync sends a single
/// batch of SELECTs (separated by ';') and reads the result sets back in order, instead of issuing
/// one query per widget. All hours/earnings math is done in SQL as decimal arithmetic
/// (DATEDIFF(SECOND, ...) cast to DECIMAL before dividing) — never float/double, and never
/// replayed per-visit in C#, since a dashboard aggregates across potentially many visits.
///
/// Every value that varies — including fixed enum comparisons like VisitStatus.Completed — is
/// passed as a Dapper parameter, never string-concatenated into the SQL text, even though the
/// enum values themselves never come from caller input.
/// </summary>
public sealed class DashboardDapperRepository : IDashboardReadRepository
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

    // DATEDIFF(SECOND, ...) is an int/bigint; casting to DECIMAL before dividing by 3600 keeps the
    // whole computation in SQL Server's decimal arithmetic, never float — same "no floating-point
    // money math" rule as CaregiverEarningsCalculator, just implemented in T-SQL for aggregate
    // reporting across many rows instead of C# for a single caregiver's payroll figure.
    private const string WorkedHoursExpression = "CAST(DATEDIFF(SECOND, v.ActualStartUtc, v.ActualEndUtc) AS DECIMAL(18,4)) / 3600.0";

    private readonly IDbConnectionFactory _connectionFactory;

    public DashboardDapperRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<AdminDashboardDto> GetAdminDashboardAsync(string requestingAuth0UserId, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var requester = await RequesterResolver.ResolveAsync(connection, requestingAuth0UserId, cancellationToken);

        if (requester.Role != UserRole.Admin)
        {
            throw new ForbiddenException("Only Admin may view the admin dashboard.");
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        const string sql = $"""
            SELECT COUNT(*) FROM Clients;

            SELECT COUNT(*) FROM Caregivers WHERE IsActive = 1;

            SELECT
                COUNT(*) AS TodaysVisitCount,
                SUM(CASE WHEN Status = @Completed THEN 1 ELSE 0 END) AS CompletedVisitsToday,
                SUM(CASE WHEN Status IN (@Scheduled, @InProgress) THEN 1 ELSE 0 END) AS PendingVisitsToday,
                SUM(CASE WHEN Status IN (@Cancelled, @NoShow) THEN 1 ELSE 0 END) AS CancelledVisitsToday
            FROM Visits
            WHERE CAST(ScheduledStartUtc AS date) = @Today;

            SELECT
                ISNULL(SUM({WorkedHoursExpression}), 0) AS TotalHoursWorked,
                ISNULL(SUM({WorkedHoursExpression} * c.HourlyRate), 0) AS TotalCaregiverEarnings
            FROM Visits v
            INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
            INNER JOIN Caregivers c ON c.Id = a.CaregiverId
            WHERE v.Status = @Completed AND v.ActualStartUtc IS NOT NULL AND v.ActualEndUtc IS NOT NULL;
            """;

        using var multi = await connection.QueryMultipleAsync(new CommandDefinition(
            sql,
            new
            {
                Today = today,
                Completed = VisitStatus.Completed,
                Scheduled = VisitStatus.Scheduled,
                InProgress = VisitStatus.InProgress,
                Cancelled = VisitStatus.Cancelled,
                NoShow = VisitStatus.NoShow,
            },
            cancellationToken: cancellationToken));

        var totalClients = await multi.ReadSingleAsync<int>();
        var activeCaregivers = await multi.ReadSingleAsync<int>();
        var todaysBreakdown = await multi.ReadSingleAsync<TodaysBreakdownRow>();
        var hoursEarnings = await multi.ReadSingleAsync<HoursEarningsRow>();

        return new AdminDashboardDto
        {
            TotalClients = totalClients,
            ActiveCaregivers = activeCaregivers,
            TodaysVisitCount = todaysBreakdown.TodaysVisitCount,
            CompletedVisitsToday = todaysBreakdown.CompletedVisitsToday,
            PendingVisitsToday = todaysBreakdown.PendingVisitsToday,
            CancelledVisitsToday = todaysBreakdown.CancelledVisitsToday,
            TotalHoursWorked = hoursEarnings.TotalHoursWorked,
            TotalCaregiverEarnings = hoursEarnings.TotalCaregiverEarnings,
        };
    }

    public async Task<CaregiverDashboardDto> GetCaregiverDashboardAsync(string requestingAuth0UserId, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var requester = await RequesterResolver.ResolveAsync(connection, requestingAuth0UserId, cancellationToken);

        if (requester.Role != UserRole.Caregiver || requester.CaregiverId is not { } caregiverId)
        {
            throw new ForbiddenException("Only a caregiver may view their own dashboard.");
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var nowUtc = DateTime.UtcNow;

        const string sql = $"""
            SELECT {VisitSummaryColumns}
            {VisitJoins}
            WHERE a.CaregiverId = @CaregiverId AND CAST(v.ScheduledStartUtc AS date) = @Today
            ORDER BY v.ScheduledStartUtc;

            SELECT {VisitSummaryColumns}
            {VisitJoins}
            WHERE a.CaregiverId = @CaregiverId AND v.Status = @Scheduled AND v.ScheduledStartUtc > @NowUtc
            ORDER BY v.ScheduledStartUtc;

            SELECT
                COUNT(*) AS CompletedVisitCount,
                ISNULL(SUM({WorkedHoursExpression}), 0) AS TotalHoursWorked,
                ISNULL(SUM({WorkedHoursExpression} * c.HourlyRate), 0) AS ActualEarnings
            FROM Visits v
            INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
            INNER JOIN Caregivers c ON c.Id = a.CaregiverId
            WHERE a.CaregiverId = @CaregiverId AND v.Status = @Completed
              AND v.ActualStartUtc IS NOT NULL AND v.ActualEndUtc IS NOT NULL;

            SELECT
                ISNULL(SUM(CAST(DATEDIFF(SECOND, v.ScheduledStartUtc, v.ScheduledEndUtc) AS DECIMAL(18,4)) / 3600.0 * c.HourlyRate), 0)
                    AS EstimatedUpcomingEarnings
            FROM Visits v
            INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
            INNER JOIN Caregivers c ON c.Id = a.CaregiverId
            WHERE a.CaregiverId = @CaregiverId AND v.Status = @Scheduled;
            """;

        using var multi = await connection.QueryMultipleAsync(new CommandDefinition(
            sql,
            new
            {
                CaregiverId = caregiverId,
                Today = today,
                NowUtc = nowUtc,
                Scheduled = VisitStatus.Scheduled,
                Completed = VisitStatus.Completed,
            },
            cancellationToken: cancellationToken));

        var todaysVisits = (await multi.ReadAsync<VisitSummaryDto>()).ToList();
        var upcomingVisits = (await multi.ReadAsync<VisitSummaryDto>()).ToList();
        var completedAndHours = await multi.ReadSingleAsync<CaregiverCompletedHoursRow>();
        var estimatedUpcoming = await multi.ReadSingleAsync<decimal>();

        return new CaregiverDashboardDto
        {
            TodaysVisits = todaysVisits,
            UpcomingVisits = upcomingVisits,
            CompletedVisitCount = completedAndHours.CompletedVisitCount,
            TotalHoursWorked = completedAndHours.TotalHoursWorked,
            ActualEarnings = completedAndHours.ActualEarnings,
            EstimatedUpcomingEarnings = estimatedUpcoming,
        };
    }

    public async Task<ClientDashboardDto> GetClientDashboardAsync(string requestingAuth0UserId, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var requester = await RequesterResolver.ResolveAsync(connection, requestingAuth0UserId, cancellationToken);

        if (requester.Role != UserRole.Client || requester.ClientId is not { } clientId)
        {
            throw new ForbiddenException("Only a client may view their own dashboard.");
        }

        var nowUtc = DateTime.UtcNow;

        const string sql = $"""
            SELECT DISTINCT c.Id AS CaregiverId, cgu.FirstName + ' ' + cgu.LastName AS FullName, cgu.PhoneNumber
            FROM CaregiverAssignments a
            INNER JOIN Caregivers c ON c.Id = a.CaregiverId
            INNER JOIN Users cgu ON cgu.Id = c.UserId
            WHERE a.ClientId = @ClientId AND a.Status = @ActiveAssignment;

            SELECT TOP (1) {VisitSummaryColumns}
            {VisitJoins}
            WHERE a.ClientId = @ClientId AND v.Status = @Scheduled AND v.ScheduledStartUtc > @NowUtc
            ORDER BY v.ScheduledStartUtc;

            SELECT {VisitSummaryColumns}
            {VisitJoins}
            WHERE a.ClientId = @ClientId AND v.Status = @Scheduled AND v.ScheduledStartUtc > @NowUtc
            ORDER BY v.ScheduledStartUtc;

            SELECT COUNT(*)
            FROM Visits v
            INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
            WHERE a.ClientId = @ClientId AND v.Status = @Completed;

            SELECT TOP (10) n.Id, n.VisitId, n.AuthorUserId, u.FirstName + ' ' + u.LastName AS AuthorFullName,
                   n.Content, n.CreatedAtUtc
            FROM VisitNotes n
            INNER JOIN Visits v ON v.Id = n.VisitId
            INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
            INNER JOIN Users u ON u.Id = n.AuthorUserId
            WHERE a.ClientId = @ClientId
            ORDER BY n.CreatedAtUtc DESC;
            """;

        using var multi = await connection.QueryMultipleAsync(new CommandDefinition(
            sql,
            new
            {
                ClientId = clientId,
                NowUtc = nowUtc,
                ActiveAssignment = AssignmentStatus.Active,
                Scheduled = VisitStatus.Scheduled,
                Completed = VisitStatus.Completed,
            },
            cancellationToken: cancellationToken));

        var assignedCaregivers = (await multi.ReadAsync<AssignedCaregiverDto>()).ToList();
        var nextVisit = await multi.ReadSingleOrDefaultAsync<VisitSummaryDto>();
        var upcomingVisits = (await multi.ReadAsync<VisitSummaryDto>()).ToList();
        var completedVisitCount = await multi.ReadSingleAsync<int>();
        var recentNotes = (await multi.ReadAsync<VisitNoteDto>()).ToList();

        return new ClientDashboardDto
        {
            AssignedCaregivers = assignedCaregivers,
            NextVisit = nextVisit,
            UpcomingVisits = upcomingVisits,
            CompletedVisitCount = completedVisitCount,
            RecentVisitNotes = recentNotes,
        };
    }

    private sealed class TodaysBreakdownRow
    {
        public int TodaysVisitCount { get; init; }

        public int CompletedVisitsToday { get; init; }

        public int PendingVisitsToday { get; init; }

        public int CancelledVisitsToday { get; init; }
    }

    private sealed class HoursEarningsRow
    {
        public decimal TotalHoursWorked { get; init; }

        public decimal TotalCaregiverEarnings { get; init; }
    }

    private sealed class CaregiverCompletedHoursRow
    {
        public int CompletedVisitCount { get; init; }

        public decimal TotalHoursWorked { get; init; }

        public decimal ActualEarnings { get; init; }
    }
}
