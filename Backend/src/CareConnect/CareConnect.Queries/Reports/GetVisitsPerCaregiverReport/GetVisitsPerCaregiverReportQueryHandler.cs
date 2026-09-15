using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Reports;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Common;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Reports.GetVisitsPerCaregiverReport;

public sealed class GetVisitsPerCaregiverReportQueryHandler : IRequestHandler<GetVisitsPerCaregiverReportQuery, IReadOnlyList<VisitsPerCaregiverReportRowDto>>
{
    private static readonly string Sql =
        SqlResourceLoader.Load(typeof(GetVisitsPerCaregiverReportQueryHandler), "GetVisitsPerCaregiverReportQuery.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public GetVisitsPerCaregiverReportQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<VisitsPerCaregiverReportRowDto>> Handle(GetVisitsPerCaregiverReportQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await ReportAccess.EnsureAdminAsync(connection, request.RequestingAuth0UserId, cancellationToken);

        var rows = await connection.QueryAsync<VisitsPerCaregiverReportRowDto>(new CommandDefinition(
            Sql,
            new
            {
                request.FromDate,
                request.ToDate,
                Completed = VisitStatus.Completed,
                Cancelled = VisitStatus.Cancelled,
                NoShow = VisitStatus.NoShow,
            },
            cancellationToken: cancellationToken));

        return rows.ToList();
    }
}
