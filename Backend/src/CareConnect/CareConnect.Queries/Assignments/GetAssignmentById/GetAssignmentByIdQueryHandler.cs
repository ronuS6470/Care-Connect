using CareConnect.DTOs.Assignments;
using CareConnect.DTOs.Enums;
using CareConnect.Infrastructure.Data;
using CareConnect.Infrastructure.Errors;
using CareConnect.Queries.Common;
using CareConnect.Queries.Security;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Assignments.GetAssignmentById;

public sealed class GetAssignmentByIdQueryHandler : IRequestHandler<GetAssignmentByIdQuery, AssignmentDto?>
{
    private static readonly string Sql =
        SqlResourceLoader.Load(typeof(GetAssignmentByIdQueryHandler), "GetAssignmentByIdQuery.sql");

    private static readonly string OwnerLookupSql =
        SqlResourceLoader.Load(typeof(GetAssignmentByIdQueryHandler), "GetAssignmentOwnerQuery.sql");

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
}
