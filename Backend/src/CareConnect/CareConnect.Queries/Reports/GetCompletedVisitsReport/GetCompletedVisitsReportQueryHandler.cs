using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Reporting;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Common;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Reports.GetCompletedVisitsReport;

public sealed class GetCompletedVisitsReportQueryHandler : IRequestHandler<GetCompletedVisitsReportQuery, IReadOnlyList<VisitSummaryDto>>
{
    private static readonly string Sql =
        SqlResourceLoader.Load(typeof(GetCompletedVisitsReportQueryHandler), "GetCompletedVisitsReportQuery.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public GetCompletedVisitsReportQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<VisitSummaryDto>> Handle(GetCompletedVisitsReportQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await ReportAccess.EnsureAdminAsync(connection, request.RequestingAuth0UserId, cancellationToken);

        var rows = await connection.QueryAsync<VisitSummaryDto>(new CommandDefinition(
            Sql,
            new { request.FromDate, request.ToDate, Completed = VisitStatus.Completed },
            cancellationToken: cancellationToken));

        return rows.ToList();
    }
}
