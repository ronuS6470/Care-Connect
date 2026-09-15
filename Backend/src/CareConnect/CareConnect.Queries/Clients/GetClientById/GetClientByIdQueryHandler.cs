using CareConnect.DTOs.Clients;
using CareConnect.DTOs.Enums;
using CareConnect.Infrastructure.Data;
using CareConnect.Infrastructure.Errors;
using CareConnect.Queries.Common;
using CareConnect.Queries.Security;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Clients.GetClientById;

public sealed class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, ClientDto?>
{
    private static readonly string Sql =
        SqlResourceLoader.Load(typeof(GetClientByIdQueryHandler), "GetClientByIdQuery.sql");

    private static readonly string CaregiverHasAssignmentSql =
        SqlResourceLoader.Load(typeof(GetClientByIdQueryHandler), "CaregiverHasClientAssignmentQuery.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public GetClientByIdQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ClientDto?> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var requester = await RequesterResolver.ResolveAsync(connection, request.RequestingAuth0UserId, cancellationToken);

        if (requester.Role == UserRole.Client && requester.ClientId != request.ClientId)
        {
            throw new ForbiddenException("You may only access your own client record.");
        }

        if (requester.Role == UserRole.Caregiver)
        {
            var hasAssignment = await connection.ExecuteScalarAsync<bool>(new CommandDefinition(
                CaregiverHasAssignmentSql,
                new { requester.CaregiverId, request.ClientId },
                cancellationToken: cancellationToken));

            if (!hasAssignment)
            {
                throw new ForbiddenException("You may only access clients related to your assignments.");
            }
        }

        return await connection.QuerySingleOrDefaultAsync<ClientDto>(new CommandDefinition(
            Sql,
            new { request.ClientId },
            cancellationToken: cancellationToken));
    }
}
