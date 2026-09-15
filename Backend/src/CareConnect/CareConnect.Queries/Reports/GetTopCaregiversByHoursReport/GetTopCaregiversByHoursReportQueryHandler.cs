using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Reports;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Common;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Reports.GetTopCaregiversByHoursReport;

public sealed class GetTopCaregiversByHoursReportQueryHandler
    : IRequestHandler<GetTopCaregiversByHoursReportQuery, IReadOnlyList<TopCaregiverByHoursReportRowDto>>
{
    private static readonly string Sql =
        SqlResourceLoader.Load(typeof(GetTopCaregiversByHoursReportQueryHandler), "GetTopCaregiversByHoursReportQuery.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public GetTopCaregiversByHoursReportQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<TopCaregiverByHoursReportRowDto>> Handle(
        GetTopCaregiversByHoursReportQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await ReportAccess.EnsureAdminAsync(connection, request.RequestingAuth0UserId, cancellationToken);

        var rows = (await connection.QueryAsync<TopCaregiverByHoursRow>(new CommandDefinition(
            Sql,
            new { request.FromDate, request.ToDate, request.TopN, Completed = VisitStatus.Completed },
            cancellationToken: cancellationToken))).ToList();

        return rows
            .Select((row, index) => new TopCaregiverByHoursReportRowDto
            {
                Rank = index + 1,
                CaregiverId = row.CaregiverId,
                CaregiverFullName = row.CaregiverFullName,
                TotalHoursWorked = row.TotalHoursWorked,
            })
            .ToList();
    }
}
