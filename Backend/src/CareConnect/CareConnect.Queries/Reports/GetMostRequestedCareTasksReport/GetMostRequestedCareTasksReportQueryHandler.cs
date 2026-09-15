using CareConnect.DTOs.Reports;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Common;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Reports.GetMostRequestedCareTasksReport;

public sealed class GetMostRequestedCareTasksReportQueryHandler : IRequestHandler<GetMostRequestedCareTasksReportQuery, IReadOnlyList<CareTaskDemandReportRowDto>>
{
    private static readonly string Sql =
        SqlResourceLoader.Load(typeof(GetMostRequestedCareTasksReportQueryHandler), "GetMostRequestedCareTasksReportQuery.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public GetMostRequestedCareTasksReportQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<CareTaskDemandReportRowDto>> Handle(GetMostRequestedCareTasksReportQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await ReportAccess.EnsureAdminAsync(connection, request.RequestingAuth0UserId, cancellationToken);

        var rows = await connection.QueryAsync<CareTaskDemandReportRowDto>(new CommandDefinition(
            Sql,
            new { request.FromDate, request.ToDate },
            cancellationToken: cancellationToken));

        return rows.ToList();
    }
}
