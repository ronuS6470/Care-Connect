using CareConnect.DTOs.Common;
using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Reporting;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Security;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Visits;

public sealed class GetVisitsQueryHandler : IRequestHandler<GetVisitsQuery, PagedResponseDto<VisitSummaryDto>>
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

        var authFilterSql = requester.Role switch
        {
            UserRole.Admin => "1 = 1",
            UserRole.Caregiver => "a.CaregiverId = @RequesterCaregiverId",
            UserRole.Client => "a.ClientId = @RequesterClientId",
            _ => "1 = 0",
        };

        var whereSql = $"""
            WHERE ({authFilterSql})
              AND (@FromDate IS NULL OR CAST(v.ScheduledStartUtc AS date) >= @FromDate)
              AND (@ToDate IS NULL OR CAST(v.ScheduledStartUtc AS date) <= @ToDate)
              AND (@CaregiverId IS NULL OR a.CaregiverId = @CaregiverId)
              AND (@ClientId IS NULL OR a.ClientId = @ClientId)
              AND (@Status IS NULL OR v.Status = @Status)
              AND (@Search IS NULL OR cgu.FirstName LIKE '%' + @Search + '%' OR cgu.LastName LIKE '%' + @Search + '%'
                   OR clu.FirstName LIKE '%' + @Search + '%' OR clu.LastName LIKE '%' + @Search + '%')
            """;

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
