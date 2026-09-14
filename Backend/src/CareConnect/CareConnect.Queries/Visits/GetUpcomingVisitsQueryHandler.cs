using CareConnect.DTOs.Common;
using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Reporting;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Security;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Visits;

/// <summary>
/// Distinct from GetTodaysVisitsQuery: "upcoming" needs a precise ScheduledStartUtc &gt;= now
/// comparison (so visits earlier today that already started are excluded), not a date-only
/// comparison — so this owns its own SQL rather than delegating to GetVisitsQuery.
/// </summary>
public sealed class GetUpcomingVisitsQueryHandler : IRequestHandler<GetUpcomingVisitsQuery, PagedResponseDto<VisitSummaryDto>>
{
    private const string Joins = """
        FROM Visits v
        INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
        INNER JOIN Caregivers cg ON cg.Id = a.CaregiverId
        INNER JOIN Users cgu ON cgu.Id = cg.UserId
        INNER JOIN Clients cl ON cl.Id = a.ClientId
        INNER JOIN Users clu ON clu.Id = cl.UserId
        """;

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

        var authFilterSql = requester.Role switch
        {
            UserRole.Admin => "1 = 1",
            UserRole.Caregiver => "a.CaregiverId = @RequesterCaregiverId",
            UserRole.Client => "a.ClientId = @RequesterClientId",
            _ => "1 = 0",
        };

        var whereSql = $"WHERE ({authFilterSql}) AND v.ScheduledStartUtc >= @NowUtc AND v.Status = @ScheduledStatus";

        var countSql = $"SELECT COUNT(*) {Joins} {whereSql};";

        var dataSql = $"""
            SELECT v.Id AS VisitId, cgu.FirstName + ' ' + cgu.LastName AS CaregiverFullName,
                   clu.FirstName + ' ' + clu.LastName AS ClientFullName,
                   v.ScheduledStartUtc, v.ScheduledEndUtc, v.Status
            {Joins}
            {whereSql}
            ORDER BY v.ScheduledStartUtc
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        var parameters = new
        {
            RequesterCaregiverId = requester.CaregiverId,
            RequesterClientId = requester.ClientId,
            NowUtc = DateTime.UtcNow,
            ScheduledStatus = VisitStatus.Scheduled,
            Offset = (page - 1) * pageSize,
            PageSize = pageSize,
        };

        var totalRecords = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            countSql, parameters, cancellationToken: cancellationToken));

        var data = await connection.QueryAsync<VisitSummaryDto>(new CommandDefinition(
            dataSql, parameters, cancellationToken: cancellationToken));

        return new PagedResponseDto<VisitSummaryDto>
        {
            Data = data.ToList(),
            CurrentPage = page,
            PageSize = pageSize,
            TotalRecords = totalRecords,
        };
    }
}
