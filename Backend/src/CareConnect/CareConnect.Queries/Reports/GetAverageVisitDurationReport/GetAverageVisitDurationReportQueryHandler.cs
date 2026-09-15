using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Reports;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Common;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Reports.GetAverageVisitDurationReport;

public sealed class GetAverageVisitDurationReportQueryHandler : IRequestHandler<GetAverageVisitDurationReportQuery, IReadOnlyList<AverageVisitDurationReportRowDto>>
{
    private static readonly string Sql =
        SqlResourceLoader.Load(typeof(GetAverageVisitDurationReportQueryHandler), "GetAverageVisitDurationReportQuery.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public GetAverageVisitDurationReportQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<AverageVisitDurationReportRowDto>> Handle(GetAverageVisitDurationReportQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await ReportAccess.EnsureAdminAsync(connection, request.RequestingAuth0UserId, cancellationToken);

        var rows = await connection.QueryAsync<AverageVisitDurationReportRowDto>(new CommandDefinition(
            Sql,
            new { request.FromDate, request.ToDate, Completed = VisitStatus.Completed },
            cancellationToken: cancellationToken));

        return rows.ToList();
    }
}
