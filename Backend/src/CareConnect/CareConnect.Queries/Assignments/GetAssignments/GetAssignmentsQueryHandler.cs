using CareConnect.DTOs.Assignments;
using CareConnect.DTOs.Common;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Common;
using CareConnect.Queries.Security;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Assignments.GetAssignments;

public sealed class GetAssignmentsQueryHandler : IRequestHandler<GetAssignmentsQuery, PagedResponseDto<AssignmentDto>>
{
    private static readonly string CountSql =
        SqlResourceLoader.Load(typeof(GetAssignmentsQueryHandler), "GetAssignmentsCountQuery.sql");

    private static readonly string DataSql =
        SqlResourceLoader.Load(typeof(GetAssignmentsQueryHandler), "GetAssignmentsQuery.sql");

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

        // The role decides which branch of the SQL's row-scoping predicate applies.
        var parameters = new
        {
            RequesterRole = requester.Role,
            RequesterCaregiverId = requester.CaregiverId,
            RequesterClientId = requester.ClientId,
            request.Status,
            Offset = (page - 1) * pageSize,
            PageSize = pageSize,
        };

        var totalRecords = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            CountSql, parameters, cancellationToken: cancellationToken));

        var data = await connection.QueryAsync<AssignmentDto>(new CommandDefinition(
            DataSql, parameters, cancellationToken: cancellationToken));

        return new PagedResponseDto<AssignmentDto>
        {
            Data = data.ToList(),
            CurrentPage = page,
            PageSize = pageSize,
            TotalRecords = totalRecords,
        };
    }
}
