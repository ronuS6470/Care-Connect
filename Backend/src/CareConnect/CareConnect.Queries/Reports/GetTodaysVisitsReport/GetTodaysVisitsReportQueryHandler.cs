using CareConnect.DTOs.Reporting;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Common;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Reports.GetTodaysVisitsReport;

public sealed class GetTodaysVisitsReportQueryHandler : IRequestHandler<GetTodaysVisitsReportQuery, IReadOnlyList<VisitSummaryDto>>
{
    private static readonly string Sql =
        SqlResourceLoader.Load(typeof(GetTodaysVisitsReportQueryHandler), "GetTodaysVisitsReportQuery.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public GetTodaysVisitsReportQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<VisitSummaryDto>> Handle(GetTodaysVisitsReportQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await ReportAccess.EnsureAdminAsync(connection, request.RequestingAuth0UserId, cancellationToken);

        var rows = await connection.QueryAsync<VisitSummaryDto>(new CommandDefinition(
            Sql,
            new { Today = DateOnly.FromDateTime(DateTime.UtcNow) },
            cancellationToken: cancellationToken));

        return rows.ToList();
    }
}
