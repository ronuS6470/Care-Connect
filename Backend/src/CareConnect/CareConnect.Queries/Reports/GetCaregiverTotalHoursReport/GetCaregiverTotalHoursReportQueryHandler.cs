using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Reports;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Common;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Reports.GetCaregiverTotalHoursReport;

public sealed class GetCaregiverTotalHoursReportQueryHandler : IRequestHandler<GetCaregiverTotalHoursReportQuery, IReadOnlyList<CaregiverHoursReportRowDto>>
{
    private static readonly string Sql =
        SqlResourceLoader.Load(typeof(GetCaregiverTotalHoursReportQueryHandler), "GetCaregiverTotalHoursReportQuery.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public GetCaregiverTotalHoursReportQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<CaregiverHoursReportRowDto>> Handle(GetCaregiverTotalHoursReportQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await ReportAccess.EnsureAdminAsync(connection, request.RequestingAuth0UserId, cancellationToken);

        var rows = await connection.QueryAsync<CaregiverHoursReportRowDto>(new CommandDefinition(
            Sql,
            new { request.FromDate, request.ToDate, Completed = VisitStatus.Completed },
            cancellationToken: cancellationToken));

        return rows.ToList();
    }
}
