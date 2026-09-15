using CareConnect.DTOs.Common;
using CareConnect.DTOs.Reporting;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Common;
using CareConnect.Queries.Security;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Visits.GetVisits;

public sealed class GetVisitsQueryHandler : IRequestHandler<GetVisitsQuery, PagedResponseDto<VisitSummaryDto>>
{
    private static readonly string CountSql =
        SqlResourceLoader.Load(typeof(GetVisitsQueryHandler), "GetVisitsCountQuery.sql");

    private static readonly string DataSql =
        SqlResourceLoader.Load(typeof(GetVisitsQueryHandler), "GetVisitsQuery.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public GetVisitsQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponseDto<VisitSummaryDto>> Handle(GetVisitsQuery request, CancellationToken cancellationToken)
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
            request.FromDate,
            request.ToDate,
            request.CaregiverId,
            request.ClientId,
            request.Status,
            Search = string.IsNullOrWhiteSpace(request.Search) ? null : request.Search.Trim(),
            Offset = (page - 1) * pageSize,
            PageSize = pageSize,
        };

        var totalRecords = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            CountSql, parameters, cancellationToken: cancellationToken));

        var data = await connection.QueryAsync<VisitSummaryDto>(new CommandDefinition(
            DataSql, parameters, cancellationToken: cancellationToken));

        return new PagedResponseDto<VisitSummaryDto>
        {
            Data = data.ToList(),
            CurrentPage = page,
            PageSize = pageSize,
            TotalRecords = totalRecords,
        };
    }
}
