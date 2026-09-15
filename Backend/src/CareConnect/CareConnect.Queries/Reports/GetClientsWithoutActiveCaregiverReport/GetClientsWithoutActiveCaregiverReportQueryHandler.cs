using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Reports;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Common;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Reports.GetClientsWithoutActiveCaregiverReport;

public sealed class GetClientsWithoutActiveCaregiverReportQueryHandler : IRequestHandler<GetClientsWithoutActiveCaregiverReportQuery, IReadOnlyList<ClientWithoutActiveCaregiverDto>>
{
    private static readonly string Sql =
        SqlResourceLoader.Load(typeof(GetClientsWithoutActiveCaregiverReportQueryHandler), "GetClientsWithoutActiveCaregiverReportQuery.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public GetClientsWithoutActiveCaregiverReportQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<ClientWithoutActiveCaregiverDto>> Handle(GetClientsWithoutActiveCaregiverReportQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await ReportAccess.EnsureAdminAsync(connection, request.RequestingAuth0UserId, cancellationToken);

        var rows = await connection.QueryAsync<ClientWithoutActiveCaregiverDto>(new CommandDefinition(
            Sql,
            new { ActiveAssignment = AssignmentStatus.Active },
            cancellationToken: cancellationToken));

        return rows.ToList();
    }
}
