using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Reports;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Common;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Reports.GetCaregiverTotalEarningsReport;

public sealed class GetCaregiverTotalEarningsReportQueryHandler : IRequestHandler<GetCaregiverTotalEarningsReportQuery, IReadOnlyList<CaregiverEarningsReportRowDto>>
{
    private static readonly string Sql =
        SqlResourceLoader.Load(typeof(GetCaregiverTotalEarningsReportQueryHandler), "GetCaregiverTotalEarningsReportQuery.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public GetCaregiverTotalEarningsReportQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<CaregiverEarningsReportRowDto>> Handle(GetCaregiverTotalEarningsReportQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await ReportAccess.EnsureAdminAsync(connection, request.RequestingAuth0UserId, cancellationToken);

        var rows = await connection.QueryAsync<CaregiverEarningsReportRowDto>(new CommandDefinition(
            Sql,
            new { request.FromDate, request.ToDate, Completed = VisitStatus.Completed },
            cancellationToken: cancellationToken));

        return rows.ToList();
    }
}
