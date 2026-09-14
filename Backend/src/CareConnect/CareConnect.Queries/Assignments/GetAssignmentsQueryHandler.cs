using CareConnect.DTOs.Assignments;
using CareConnect.DTOs.Common;
using CareConnect.DTOs.Enums;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Security;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Assignments;

public sealed class GetAssignmentsQueryHandler : IRequestHandler<GetAssignmentsQuery, PagedResponseDto<AssignmentDto>>
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

    private readonly IDbConnectionFactory _connectionFactory;

    public GetAssignmentsQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponseDto<AssignmentDto>> Handle(GetAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        using var connection = _connectionFactory.CreateConnection();

        var requester = await RequesterResolver.ResolveAsync(connection, request.RequestingAuth0UserId, cancellationToken);

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
            request.Status,
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
}
