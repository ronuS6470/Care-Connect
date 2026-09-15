using CareConnect.DTOs.Dashboards;
using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Reporting;
using CareConnect.Infrastructure.Data;
using CareConnect.Infrastructure.Errors;
using CareConnect.Queries.Common;
using CareConnect.Queries.Security;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Dashboards.GetCaregiverDashboard;

public sealed class GetCaregiverDashboardQueryHandler : IRequestHandler<GetCaregiverDashboardQuery, CaregiverDashboardDto>
{
    private static readonly string Sql =
        SqlResourceLoader.Load(typeof(GetCaregiverDashboardQueryHandler), "GetCaregiverDashboardQuery.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public GetCaregiverDashboardQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<CaregiverDashboardDto> Handle(GetCaregiverDashboardQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var requester = await RequesterResolver.ResolveAsync(connection, request.RequestingAuth0UserId, cancellationToken);

        if (requester.Role != UserRole.Caregiver || requester.CaregiverId is not { } caregiverId)
        {
            throw new ForbiddenException("Only a caregiver may view their own dashboard.");
        }

        using var multi = await connection.QueryMultipleAsync(new CommandDefinition(
            Sql,
            new
            {
                CaregiverId = caregiverId,
                Today = DateOnly.FromDateTime(DateTime.UtcNow),
                NowUtc = DateTime.UtcNow,
                Scheduled = VisitStatus.Scheduled,
                Completed = VisitStatus.Completed,
            },
            cancellationToken: cancellationToken));

        // Read in the order the SELECTs appear in GetCaregiverDashboardQuery.sql.
        var todaysVisits = (await multi.ReadAsync<VisitSummaryDto>()).ToList();
        var upcomingVisits = (await multi.ReadAsync<VisitSummaryDto>()).ToList();
        var completedAndHours = await multi.ReadSingleAsync<CaregiverCompletedHoursRow>();
        var estimatedUpcoming = await multi.ReadSingleAsync<decimal>();

        return new CaregiverDashboardDto
        {
            TodaysVisits = todaysVisits,
            UpcomingVisits = upcomingVisits,
            CompletedVisitCount = completedAndHours.CompletedVisitCount,
            TotalHoursWorked = completedAndHours.TotalHoursWorked,
            ActualEarnings = completedAndHours.ActualEarnings,
            EstimatedUpcomingEarnings = estimatedUpcoming,
        };
    }
}
