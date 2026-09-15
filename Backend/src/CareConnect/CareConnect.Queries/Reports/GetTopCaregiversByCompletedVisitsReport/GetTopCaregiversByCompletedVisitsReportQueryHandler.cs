using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Reports;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Common;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Reports.GetTopCaregiversByCompletedVisitsReport;

public sealed class GetTopCaregiversByCompletedVisitsReportQueryHandler : IRequestHandler<GetTopCaregiversByCompletedVisitsReportQuery, IReadOnlyList<TopCompletedVisitsCaregiverReportRowDto>>
{
    private static readonly string Sql =
        SqlResourceLoader.Load(typeof(GetTopCaregiversByCompletedVisitsReportQueryHandler), "GetTopCaregiversByCompletedVisitsReportQuery.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public GetTopCaregiversByCompletedVisitsReportQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<TopCompletedVisitsCaregiverReportRowDto>> Handle(GetTopCaregiversByCompletedVisitsReportQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await ReportAccess.EnsureAdminAsync(connection, request.RequestingAuth0UserId, cancellationToken);

        var rows = await connection.QueryAsync<TopCompletedVisitsCaregiverReportRowDto>(new CommandDefinition(
            Sql,
            new { request.FromDate, request.ToDate, request.TopN, Completed = VisitStatus.Completed },
            cancellationToken: cancellationToken));

        return rows.ToList();
    }
}
