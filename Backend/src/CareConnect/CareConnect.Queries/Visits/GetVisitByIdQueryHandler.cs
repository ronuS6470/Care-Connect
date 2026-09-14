using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Errors;
using CareConnect.DTOs.Visits;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Security;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Visits;

public sealed class GetVisitByIdQueryHandler : IRequestHandler<GetVisitByIdQuery, VisitDto?>
{
    private const string VisitSql = """
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

    private readonly IDbConnectionFactory _connectionFactory;

    public GetVisitByIdQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<VisitDto?> Handle(GetVisitByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var requester = await RequesterResolver.ResolveAsync(connection, request.RequestingAuth0UserId, cancellationToken);

        var row = await connection.QuerySingleOrDefaultAsync<VisitRow>(new CommandDefinition(
            VisitSql,
            new { request.VisitId },
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
            new { request.VisitId },
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
