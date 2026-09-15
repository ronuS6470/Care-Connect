using CareConnect.DTOs.Common;
using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Reporting;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Common;
using CareConnect.Queries.Security;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Visits.GetUpcomingVisits;

public sealed class GetUpcomingVisitsQueryHandler : IRequestHandler<GetUpcomingVisitsQuery, PagedResponseDto<VisitSummaryDto>>
{
    private static readonly string CountSql =
        SqlResourceLoader.Load(typeof(GetUpcomingVisitsQueryHandler), "GetUpcomingVisitsCountQuery.sql");

    private static readonly string DataSql =
        SqlResourceLoader.Load(typeof(GetUpcomingVisitsQueryHandler), "GetUpcomingVisitsQuery.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public GetUpcomingVisitsQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponseDto<VisitSummaryDto>> Handle(GetUpcomingVisitsQuery request, CancellationToken cancellationToken)
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
            NowUtc = DateTime.UtcNow,
            ScheduledStatus = VisitStatus.Scheduled,
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
