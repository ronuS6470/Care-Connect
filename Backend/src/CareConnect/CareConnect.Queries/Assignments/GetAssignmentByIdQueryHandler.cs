using CareConnect.DTOs.Assignments;
using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Errors;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Security;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Assignments;

public sealed class GetAssignmentByIdQueryHandler : IRequestHandler<GetAssignmentByIdQuery, AssignmentDto?>
{
    private const string Sql = """
        SELECT a.Id, a.CaregiverId, cgu.FirstName + ' ' + cgu.LastName AS CaregiverFullName,
               a.ClientId, clu.FirstName + ' ' + clu.LastName AS ClientFullName,
               a.Status, a.StartDate, a.EndDate, a.Notes
        FROM CaregiverAssignments a
        INNER JOIN Caregivers cg ON cg.Id = a.CaregiverId
        INNER JOIN Users cgu ON cgu.Id = cg.UserId
        INNER JOIN Clients cl ON cl.Id = a.ClientId
        INNER JOIN Users clu ON clu.Id = cl.UserId
        WHERE a.Id = @AssignmentId;
        """;

    private const string OwnerLookupSql = "SELECT CaregiverId, ClientId FROM CaregiverAssignments WHERE Id = @AssignmentId;";

    private readonly IDbConnectionFactory _connectionFactory;

    public GetAssignmentByIdQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<AssignmentDto?> Handle(GetAssignmentByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var requester = await RequesterResolver.ResolveAsync(connection, request.RequestingAuth0UserId, cancellationToken);

        if (requester.Role is UserRole.Caregiver or UserRole.Client)
        {
            var owner = await connection.QuerySingleOrDefaultAsync<AssignmentOwnerRow>(new CommandDefinition(
                OwnerLookupSql,
                new { request.AssignmentId },
                cancellationToken: cancellationToken));

            if (owner is null)
            {
                return null;
            }

            var isOwner = requester.Role == UserRole.Caregiver
                ? requester.CaregiverId == owner.CaregiverId
                : requester.ClientId == owner.ClientId;

            if (!isOwner)
            {
                throw new ForbiddenException("You may only access your own assignments.");
            }
        }

        return await connection.QuerySingleOrDefaultAsync<AssignmentDto>(new CommandDefinition(
            Sql,
            new { request.AssignmentId },
            cancellationToken: cancellationToken));
    }

    private sealed class AssignmentOwnerRow
    {
        public int CaregiverId { get; init; }

        public int ClientId { get; init; }
    }
}
