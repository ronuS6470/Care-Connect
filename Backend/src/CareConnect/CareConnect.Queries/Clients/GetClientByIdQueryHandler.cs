using CareConnect.DTOs.Clients;
using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Errors;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Security;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Clients;

public sealed class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, ClientDto?>
{
    private const string Sql = """
        SELECT c.Id, c.UserId, u.FirstName + ' ' + u.LastName AS FullName, u.Email, u.PhoneNumber,
               c.AddressLine1, c.AddressLine2, c.City, c.State, c.PostalCode,
               c.EmergencyContactName, c.EmergencyContactPhone, c.IsActive
        FROM Clients c
        INNER JOIN Users u ON u.Id = c.UserId
        WHERE c.Id = @ClientId;
        """;

    private const string CaregiverHasAssignmentSql = """
        SELECT CASE WHEN EXISTS (
            SELECT 1 FROM CaregiverAssignments WHERE CaregiverId = @CaregiverId AND ClientId = @ClientId
        ) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END;
        """;

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
