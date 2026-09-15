using CareConnect.DTOs.Dashboards;
using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Reporting;
using CareConnect.DTOs.Visits;
using CareConnect.Infrastructure.Data;
using CareConnect.Infrastructure.Errors;
using CareConnect.Queries.Common;
using CareConnect.Queries.Security;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Dashboards.GetClientDashboard;

public sealed class GetClientDashboardQueryHandler : IRequestHandler<GetClientDashboardQuery, ClientDashboardDto>
{
    private static readonly string Sql =
        SqlResourceLoader.Load(typeof(GetClientDashboardQueryHandler), "GetClientDashboardQuery.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public GetClientDashboardQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ClientDashboardDto> Handle(GetClientDashboardQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var requester = await RequesterResolver.ResolveAsync(connection, request.RequestingAuth0UserId, cancellationToken);

        if (requester.Role != UserRole.Client || requester.ClientId is not { } clientId)
        {
            throw new ForbiddenException("Only a client may view their own dashboard.");
        }

        using var multi = await connection.QueryMultipleAsync(new CommandDefinition(
            Sql,
            new
            {
                ClientId = clientId,
                NowUtc = DateTime.UtcNow,
                ActiveAssignment = AssignmentStatus.Active,
                Scheduled = VisitStatus.Scheduled,
                Completed = VisitStatus.Completed,
            },
            cancellationToken: cancellationToken));

        // Read in the order the SELECTs appear in GetClientDashboardQuery.sql.
        var assignedCaregivers = (await multi.ReadAsync<AssignedCaregiverDto>()).ToList();
        var nextVisit = await multi.ReadSingleOrDefaultAsync<VisitSummaryDto>();
        var upcomingVisits = (await multi.ReadAsync<VisitSummaryDto>()).ToList();
        var completedVisitCount = await multi.ReadSingleAsync<int>();
        var recentNotes = (await multi.ReadAsync<VisitNoteDto>()).ToList();

        return new ClientDashboardDto
        {
            AssignedCaregivers = assignedCaregivers,
            NextVisit = nextVisit,
            UpcomingVisits = upcomingVisits,
            CompletedVisitCount = completedVisitCount,
            RecentVisitNotes = recentNotes,
        };
    }
}
