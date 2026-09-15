using CareConnect.DTOs.Common;
using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Reporting;
using CareConnect.DTOs.Visits;
using CareConnect.Infrastructure.Data;
using CareConnect.Infrastructure.Errors;
using CareConnect.Queries.Security;
using Dapper;

namespace CareConnect.Queries.Visits.Repositories;

public sealed class VisitDapperRepository : IVisitReadRepository
{
    private const string Joins = """
        FROM Visits v
        INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
        INNER JOIN Caregivers cg ON cg.Id = a.CaregiverId
        INNER JOIN Users cgu ON cgu.Id = cg.UserId
        INNER JOIN Clients cl ON cl.Id = a.ClientId
        INNER JOIN Users clu ON clu.Id = cl.UserId
        """;

    private const string SummaryColumns = """
        v.Id AS VisitId, cgu.FirstName + ' ' + cgu.LastName AS CaregiverFullName,
        clu.FirstName + ' ' + clu.LastName AS ClientFullName,
        v.ScheduledStartUtc, v.ScheduledEndUtc, v.Status
        """;

    private const string VisitByIdSql = """
        SELECT v.Id, v.CaregiverAssignmentId, a.CaregiverId, a.ClientId,
               cgu.FirstName + ' ' + cgu.LastName AS CaregiverFullName,
               clu.FirstName + ' ' + clu.LastName AS ClientFullName,
               v.ScheduledStartUtc, v.ScheduledEndUtc, v.ActualStartUtc, v.ActualEndUtc,
               v.Status, v.CancellationReason
        FROM Visits v
        INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
        INNER JOIN Caregivers cg ON cg.Id = a.CaregiverId
        INNER JOIN Users cgu ON cgu.Id = cg.UserId
        INNER JOIN Clients cl ON cl.Id = a.ClientId
        INNER JOIN Users clu ON clu.Id = cl.UserId
        WHERE v.Id = @VisitId;
        """;

    private const string VisitTasksSql = """
        SELECT vt.Id, vt.VisitId, vt.CareTaskId, ct.Name AS CareTaskName,
               vt.IsCompleted, vt.CompletedAtUtc, vt.Notes
        FROM VisitTasks vt
        INNER JOIN CareTasks ct ON ct.Id = vt.CareTaskId
        WHERE vt.VisitId = @VisitId
        ORDER BY vt.Id;
        """;

    private const string VisitNotesSql = """
        SELECT n.Id, n.VisitId, n.AuthorUserId, u.FirstName + ' ' + u.LastName AS AuthorFullName,
               n.Content, n.CreatedAtUtc
        FROM VisitNotes n
        INNER JOIN Users u ON u.Id = n.AuthorUserId
        WHERE n.VisitId = @VisitId
        ORDER BY n.CreatedAtUtc;
        """;

    private readonly IDbConnectionFactory _connectionFactory;

    public VisitDapperRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponseDto<VisitSummaryDto>> GetPagedAsync(
        int page,
        int pageSize,
        DateOnly? fromDate,
        DateOnly? toDate,
        int? caregiverId,
        int? clientId,
        VisitStatus? status,
        string? search,
        string requestingAuth0UserId,
        CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var requester = await RequesterResolver.ResolveAsync(connection, requestingAuth0UserId, cancellationToken);

        var authFilterSql = requester.Role switch
        {
            UserRole.Admin => "1 = 1",
            UserRole.Caregiver => "a.CaregiverId = @RequesterCaregiverId",
            UserRole.Client => "a.ClientId = @RequesterClientId",
            _ => "1 = 0",
        };

        var whereSql = $"""
            WHERE ({authFilterSql})
              AND (@FromDate IS NULL OR CAST(v.ScheduledStartUtc AS date) >= @FromDate)
              AND (@ToDate IS NULL OR CAST(v.ScheduledStartUtc AS date) <= @ToDate)
              AND (@CaregiverId IS NULL OR a.CaregiverId = @CaregiverId)
              AND (@ClientId IS NULL OR a.ClientId = @ClientId)
              AND (@Status IS NULL OR v.Status = @Status)
              AND (@Search IS NULL OR cgu.FirstName LIKE '%' + @Search + '%' OR cgu.LastName LIKE '%' + @Search + '%'
                   OR clu.FirstName LIKE '%' + @Search + '%' OR clu.LastName LIKE '%' + @Search + '%')
            """;

        var countSql = $"SELECT COUNT(*) {Joins} {whereSql};";

        var dataSql = $"""
            SELECT {SummaryColumns}
            {Joins}
            {whereSql}
            ORDER BY v.ScheduledStartUtc
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        var parameters = new
        {
            RequesterCaregiverId = requester.CaregiverId,
            RequesterClientId = requester.ClientId,
            FromDate = fromDate,
            ToDate = toDate,
            CaregiverId = caregiverId,
            ClientId = clientId,
            Status = status,
            Search = string.IsNullOrWhiteSpace(search) ? null : search.Trim(),
            Offset = (page - 1) * pageSize,
            PageSize = pageSize,
        };

        var totalRecords = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            countSql, parameters, cancellationToken: cancellationToken));

        var data = await connection.QueryAsync<VisitSummaryDto>(new CommandDefinition(
            dataSql, parameters, cancellationToken: cancellationToken));

        return new PagedResponseDto<VisitSummaryDto>
        {
            Data = data.ToList(),
            CurrentPage = page,
            PageSize = pageSize,
            TotalRecords = totalRecords,
        };
    }

    public async Task<PagedResponseDto<VisitSummaryDto>> GetUpcomingAsync(
        int page, int pageSize, string requestingAuth0UserId, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var requester = await RequesterResolver.ResolveAsync(connection, requestingAuth0UserId, cancellationToken);

        var authFilterSql = requester.Role switch
        {
            UserRole.Admin => "1 = 1",
            UserRole.Caregiver => "a.CaregiverId = @RequesterCaregiverId",
            UserRole.Client => "a.ClientId = @RequesterClientId",
            _ => "1 = 0",
        };

        var whereSql = $"WHERE ({authFilterSql}) AND v.ScheduledStartUtc >= @NowUtc AND v.Status = @ScheduledStatus";

        var countSql = $"SELECT COUNT(*) {Joins} {whereSql};";

        var dataSql = $"""
            SELECT {SummaryColumns}
            {Joins}
            {whereSql}
            ORDER BY v.ScheduledStartUtc
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        var parameters = new
        {
            RequesterCaregiverId = requester.CaregiverId,
            RequesterClientId = requester.ClientId,
            NowUtc = DateTime.UtcNow,
            ScheduledStatus = VisitStatus.Scheduled,
            Offset = (page - 1) * pageSize,
            PageSize = pageSize,
        };

        var totalRecords = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            countSql, parameters, cancellationToken: cancellationToken));

        var data = await connection.QueryAsync<VisitSummaryDto>(new CommandDefinition(
            dataSql, parameters, cancellationToken: cancellationToken));

        return new PagedResponseDto<VisitSummaryDto>
        {
            Data = data.ToList(),
            CurrentPage = page,
            PageSize = pageSize,
            TotalRecords = totalRecords,
        };
    }

    public async Task<VisitDto?> GetByIdAsync(int visitId, string requestingAuth0UserId, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var requester = await RequesterResolver.ResolveAsync(connection, requestingAuth0UserId, cancellationToken);

        var row = await connection.QuerySingleOrDefaultAsync<VisitRow>(new CommandDefinition(
            VisitByIdSql,
            new { VisitId = visitId },
            cancellationToken: cancellationToken));

        if (row is null)
        {
            return null;
        }

        var isOwner = requester.Role switch
        {
            UserRole.Admin => true,
            UserRole.Caregiver => requester.CaregiverId == row.CaregiverId,
            UserRole.Client => requester.ClientId == row.ClientId,
            _ => false,
        };

        if (!isOwner)
        {
            throw new ForbiddenException("You may only access your own visits.");
        }

        var tasks = await connection.QueryAsync<VisitTaskDto>(new CommandDefinition(
            VisitTasksSql,
            new { VisitId = visitId },
            cancellationToken: cancellationToken));

        return new VisitDto
        {
            Id = row.Id,
            CaregiverAssignmentId = row.CaregiverAssignmentId,
            CaregiverFullName = row.CaregiverFullName,
            ClientFullName = row.ClientFullName,
            ScheduledStartUtc = row.ScheduledStartUtc,
            ScheduledEndUtc = row.ScheduledEndUtc,
            ActualStartUtc = row.ActualStartUtc,
            ActualEndUtc = row.ActualEndUtc,
            Status = row.Status,
            CancellationReason = row.CancellationReason,
            VisitTasks = tasks.ToList(),
        };
    }

    public async Task<IReadOnlyList<VisitTaskDto>> GetTasksAsync(int visitId, string requestingAuth0UserId, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var requester = await RequesterResolver.ResolveAsync(connection, requestingAuth0UserId, cancellationToken);
        await VisitAccessResolver.EnsureCanViewVisitAsync(connection, visitId, requester, cancellationToken);

        var tasks = await connection.QueryAsync<VisitTaskDto>(new CommandDefinition(
            VisitTasksSql,
            new { VisitId = visitId },
            cancellationToken: cancellationToken));

        return tasks.ToList();
    }

    public async Task<IReadOnlyList<VisitNoteDto>> GetNotesAsync(int visitId, string requestingAuth0UserId, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var requester = await RequesterResolver.ResolveAsync(connection, requestingAuth0UserId, cancellationToken);
        await VisitAccessResolver.EnsureCanViewVisitAsync(connection, visitId, requester, cancellationToken);

        var notes = await connection.QueryAsync<VisitNoteDto>(new CommandDefinition(
            VisitNotesSql,
            new { VisitId = visitId },
            cancellationToken: cancellationToken));

        return notes.ToList();
    }

    private sealed class VisitRow
    {
        public int Id { get; init; }

        public int CaregiverAssignmentId { get; init; }

        public int CaregiverId { get; init; }

        public int ClientId { get; init; }

        public string CaregiverFullName { get; init; } = string.Empty;

        public string ClientFullName { get; init; } = string.Empty;

        public DateTime ScheduledStartUtc { get; init; }

        public DateTime ScheduledEndUtc { get; init; }

        public DateTime? ActualStartUtc { get; init; }

        public DateTime? ActualEndUtc { get; init; }

        public VisitStatus Status { get; init; }

        public string? CancellationReason { get; init; }
    }
}
