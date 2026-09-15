using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Visits;
using CareConnect.Infrastructure.Data;
using CareConnect.Infrastructure.Errors;
using CareConnect.Queries.Common;
using CareConnect.Queries.Security;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Visits.GetVisitById;

public sealed class GetVisitByIdQueryHandler : IRequestHandler<GetVisitByIdQuery, VisitDto?>
{
    private static readonly string Sql =
        SqlResourceLoader.Load(typeof(GetVisitByIdQueryHandler), "GetVisitByIdQuery.sql");

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
            Sql,
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
            VisitsSharedSql.TasksByVisitId,
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
}
