using CareConnect.DTOs.Reports;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Common;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Reports.GetCaregiversWithoutVisitsTodayReport;

public sealed class GetCaregiversWithoutVisitsTodayReportQueryHandler : IRequestHandler<GetCaregiversWithoutVisitsTodayReportQuery, IReadOnlyList<CaregiverWithoutVisitsTodayDto>>
{
    private static readonly string Sql =
        SqlResourceLoader.Load(typeof(GetCaregiversWithoutVisitsTodayReportQueryHandler), "GetCaregiversWithoutVisitsTodayReportQuery.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public GetCaregiversWithoutVisitsTodayReportQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<CaregiverWithoutVisitsTodayDto>> Handle(GetCaregiversWithoutVisitsTodayReportQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await ReportAccess.EnsureAdminAsync(connection, request.RequestingAuth0UserId, cancellationToken);

        var rows = await connection.QueryAsync<CaregiverWithoutVisitsTodayDto>(new CommandDefinition(
            Sql,
            new { Today = DateOnly.FromDateTime(DateTime.UtcNow) },
            cancellationToken: cancellationToken));

        return rows.ToList();
    }
}
