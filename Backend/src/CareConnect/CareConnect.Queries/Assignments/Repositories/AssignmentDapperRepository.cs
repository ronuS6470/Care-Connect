using CareConnect.DTOs.Assignments;
using CareConnect.DTOs.Common;
using CareConnect.DTOs.Enums;
using CareConnect.Infrastructure.Data;
using CareConnect.Infrastructure.Errors;
using CareConnect.Queries.Security;
using Dapper;

namespace CareConnect.Queries.Assignments.Repositories;

public sealed class AssignmentDapperRepository : IAssignmentReadRepository
{
    private const string SelectColumns = """
        a.Id, a.CaregiverId, cgu.FirstName + ' ' + cgu.LastName AS CaregiverFullName,
        a.ClientId, clu.FirstName + ' ' + clu.LastName AS ClientFullName,
        a.Status, a.StartDate, a.EndDate, a.Notes
        """;

    private const string Joins = """
        FROM CaregiverAssignments a
        INNER JOIN Caregivers cg ON cg.Id = a.CaregiverId
        INNER JOIN Users cgu ON cgu.Id = cg.UserId
        INNER JOIN Clients cl ON cl.Id = a.ClientId
        INNER JOIN Users clu ON clu.Id = cl.UserId
        """;

    private const string ByIdSql = $"""
        SELECT {SelectColumns}
        {Joins}
        WHERE a.Id = @AssignmentId;
        """;

    private const string OwnerLookupSql = "SELECT CaregiverId, ClientId FROM CaregiverAssignments WHERE Id = @AssignmentId;";

    private readonly IDbConnectionFactory _connectionFactory;

    public AssignmentDapperRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponseDto<AssignmentDto>> GetPagedAsync(
        int page, int pageSize, AssignmentStatus? status, string requestingAuth0UserId, CancellationToken cancellationToken)
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

        var whereSql = $"WHERE ({authFilterSql}) AND (@Status IS NULL OR a.Status = @Status)";

        var countSql = $"SELECT COUNT(*) {Joins} {whereSql};";

        var dataSql = $"""
            SELECT {SelectColumns}
            {Joins}
            {whereSql}
            ORDER BY a.Id
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        var parameters = new
        {
            RequesterCaregiverId = requester.CaregiverId,
            RequesterClientId = requester.ClientId,
            Status = status,
            Offset = (page - 1) * pageSize,
            PageSize = pageSize,
        };

        var totalRecords = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            countSql, parameters, cancellationToken: cancellationToken));

        var data = await connection.QueryAsync<AssignmentDto>(new CommandDefinition(
            dataSql, parameters, cancellationToken: cancellationToken));

        return new PagedResponseDto<AssignmentDto>
        {
            Data = data.ToList(),
            CurrentPage = page,
            PageSize = pageSize,
            TotalRecords = totalRecords,
        };
    }

    public async Task<AssignmentDto?> GetByIdAsync(int assignmentId, string requestingAuth0UserId, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var requester = await RequesterResolver.ResolveAsync(connection, requestingAuth0UserId, cancellationToken);

        if (requester.Role is UserRole.Caregiver or UserRole.Client)
        {
            var owner = await connection.QuerySingleOrDefaultAsync<AssignmentOwnerRow>(new CommandDefinition(
                OwnerLookupSql,
                new { AssignmentId = assignmentId },
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
            ByIdSql,
            new { AssignmentId = assignmentId },
            cancellationToken: cancellationToken));
    }

    private sealed class AssignmentOwnerRow
    {
        public int CaregiverId { get; init; }

        public int ClientId { get; init; }
    }
}
