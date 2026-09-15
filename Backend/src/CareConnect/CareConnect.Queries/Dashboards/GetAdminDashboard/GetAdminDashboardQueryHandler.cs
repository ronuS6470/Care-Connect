using CareConnect.DTOs.Dashboards;
using CareConnect.DTOs.Enums;
using CareConnect.Infrastructure.Data;
using CareConnect.Infrastructure.Errors;
using CareConnect.Queries.Common;
using CareConnect.Queries.Security;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Dashboards.GetAdminDashboard;

/// <summary>
/// Every varying value — including fixed enum comparisons like VisitStatus.Completed — is passed as
/// a Dapper parameter, never concatenated into the SQL text, even though the enum values never come
/// from caller input.
/// </summary>
public sealed class GetAdminDashboardQueryHandler : IRequestHandler<GetAdminDashboardQuery, AdminDashboardDto>
{
    private static readonly string Sql =
        SqlResourceLoader.Load(typeof(GetAdminDashboardQueryHandler), "GetAdminDashboardQuery.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public GetAdminDashboardQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<AdminDashboardDto> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var requester = await RequesterResolver.ResolveAsync(connection, request.RequestingAuth0UserId, cancellationToken);

        if (requester.Role != UserRole.Admin)
        {
            throw new ForbiddenException("Only Admin may view the admin dashboard.");
        }

        using var multi = await connection.QueryMultipleAsync(new CommandDefinition(
            Sql,
            new
            {
                Today = DateOnly.FromDateTime(DateTime.UtcNow),
                Completed = VisitStatus.Completed,
                Scheduled = VisitStatus.Scheduled,
                InProgress = VisitStatus.InProgress,
                Cancelled = VisitStatus.Cancelled,
                NoShow = VisitStatus.NoShow,
            },
            cancellationToken: cancellationToken));

        // Read in the order the SELECTs appear in GetAdminDashboardQuery.sql.
        var totalClients = await multi.ReadSingleAsync<int>();
        var activeCaregivers = await multi.ReadSingleAsync<int>();
        var todaysBreakdown = await multi.ReadSingleAsync<TodaysBreakdownRow>();
        var hoursEarnings = await multi.ReadSingleAsync<HoursEarningsRow>();

        return new AdminDashboardDto
        {
            TotalClients = totalClients,
            ActiveCaregivers = activeCaregivers,
            TodaysVisitCount = todaysBreakdown.TodaysVisitCount,
            CompletedVisitsToday = todaysBreakdown.CompletedVisitsToday,
            PendingVisitsToday = todaysBreakdown.PendingVisitsToday,
            CancelledVisitsToday = todaysBreakdown.CancelledVisitsToday,
            TotalHoursWorked = hoursEarnings.TotalHoursWorked,
            TotalCaregiverEarnings = hoursEarnings.TotalCaregiverEarnings,
        };
    }
}
