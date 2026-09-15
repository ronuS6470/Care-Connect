using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Reports;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Common;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Reports.GetVisitsPerClientReport;

public sealed class GetVisitsPerClientReportQueryHandler : IRequestHandler<GetVisitsPerClientReportQuery, IReadOnlyList<VisitsPerClientReportRowDto>>
{
    private static readonly string Sql =
        SqlResourceLoader.Load(typeof(GetVisitsPerClientReportQueryHandler), "GetVisitsPerClientReportQuery.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public GetVisitsPerClientReportQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<VisitsPerClientReportRowDto>> Handle(GetVisitsPerClientReportQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await ReportAccess.EnsureAdminAsync(connection, request.RequestingAuth0UserId, cancellationToken);

        var rows = await connection.QueryAsync<VisitsPerClientReportRowDto>(new CommandDefinition(
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
