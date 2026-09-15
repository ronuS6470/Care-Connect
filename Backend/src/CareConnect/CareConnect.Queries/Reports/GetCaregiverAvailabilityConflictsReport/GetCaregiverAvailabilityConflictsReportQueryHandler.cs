using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Reports;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Common;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Reports.GetCaregiverAvailabilityConflictsReport;

public sealed class GetCaregiverAvailabilityConflictsReportQueryHandler
    : IRequestHandler<GetCaregiverAvailabilityConflictsReportQuery, IReadOnlyList<CaregiverAvailabilityConflictDto>>
{
    private static readonly string Sql =
        SqlResourceLoader.Load(typeof(GetCaregiverAvailabilityConflictsReportQueryHandler), "GetCaregiverAvailabilityConflictsReportQuery.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public GetCaregiverAvailabilityConflictsReportQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<CaregiverAvailabilityConflictDto>> Handle(
        GetCaregiverAvailabilityConflictsReportQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await ReportAccess.EnsureAdminAsync(connection, request.RequestingAuth0UserId, cancellationToken);

        using var multi = await connection.QueryMultipleAsync(new CommandDefinition(
            Sql,
            new { Scheduled = VisitStatus.Scheduled, InProgress = VisitStatus.InProgress },
            cancellationToken: cancellationToken));

        // Read in the order the SELECTs appear in GetCaregiverAvailabilityConflictsReportQuery.sql.
        var activeVisits = (await multi.ReadAsync<ActiveVisitRow>()).ToList();
        var overlaps = (await multi.ReadAsync<OverlapRow>()).ToList();
        var availabilityByCaregiver = (await multi.ReadAsync<AvailabilityWindowRow>()).ToLookup(w => w.CaregiverId);

        var conflicts = new List<CaregiverAvailabilityConflictDto>();

        foreach (var visit in activeVisits)
        {
            // Day-of-week comparison happens here in C#, not SQL — see the .sql file header.
            var dayOfWeek = visit.ScheduledStartUtc.DayOfWeek;
            var startTime = TimeOnly.FromDateTime(visit.ScheduledStartUtc);
            var endTime = TimeOnly.FromDateTime(visit.ScheduledEndUtc);

            var coveredByAvailability = availabilityByCaregiver[visit.CaregiverId].Any(w =>
                w.DayOfWeek == dayOfWeek && w.StartTime <= startTime && w.EndTime >= endTime);

            if (!coveredByAvailability)
            {
                conflicts.Add(new CaregiverAvailabilityConflictDto
                {
                    ConflictType = "OutsideAvailability",
                    CaregiverId = visit.CaregiverId,
                    CaregiverFullName = visit.CaregiverFullName,
                    VisitId = visit.VisitId,
                    ScheduledStartUtc = visit.ScheduledStartUtc,
                    ScheduledEndUtc = visit.ScheduledEndUtc,
                    ConflictingVisitId = null,
                    Details = "No active availability window covers this visit's day/time.",
                });
            }
        }

        foreach (var overlap in overlaps)
        {
            conflicts.Add(new CaregiverAvailabilityConflictDto
            {
                ConflictType = "OverlappingVisits",
                CaregiverId = overlap.CaregiverId,
                CaregiverFullName = overlap.CaregiverFullName,
                VisitId = overlap.VisitId,
                ScheduledStartUtc = overlap.ScheduledStartUtc,
                ScheduledEndUtc = overlap.ScheduledEndUtc,
                ConflictingVisitId = overlap.ConflictingVisitId,
                Details = $"Overlaps with visit #{overlap.ConflictingVisitId} for the same caregiver.",
            });
        }

        return conflicts;
    }
}
