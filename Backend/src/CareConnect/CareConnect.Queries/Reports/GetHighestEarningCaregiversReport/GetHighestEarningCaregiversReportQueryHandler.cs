using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Reports;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Common;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Reports.GetHighestEarningCaregiversReport;

public sealed class GetHighestEarningCaregiversReportQueryHandler : IRequestHandler<GetHighestEarningCaregiversReportQuery, IReadOnlyList<HighestEarningCaregiverReportRowDto>>
{
    private static readonly string Sql =
        SqlResourceLoader.Load(typeof(GetHighestEarningCaregiversReportQueryHandler), "GetHighestEarningCaregiversReportQuery.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public GetHighestEarningCaregiversReportQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<HighestEarningCaregiverReportRowDto>> Handle(GetHighestEarningCaregiversReportQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await ReportAccess.EnsureAdminAsync(connection, request.RequestingAuth0UserId, cancellationToken);

        var rows = await connection.QueryAsync<HighestEarningCaregiverReportRowDto>(new CommandDefinition(
            Sql,
            new { request.FromDate, request.ToDate, request.TopN, Completed = VisitStatus.Completed },
            cancellationToken: cancellationToken));

        return rows.ToList();
    }
}
