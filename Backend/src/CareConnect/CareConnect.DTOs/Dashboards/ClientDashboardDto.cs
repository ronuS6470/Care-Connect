using CareConnect.DTOs.Reporting;
using CareConnect.DTOs.Visits;

namespace CareConnect.DTOs.Dashboards;

/// <summary>
/// A client's own dashboard — always scoped to the authenticated client. RecentVisitNotes is
/// "authorized" by construction: it only ever draws from notes on visits tied to this client's own
/// assignments (the same relationship VisitAccessResolver checks elsewhere), capped to the most
/// recent entries rather than the client's full note history.
/// </summary>
public sealed class ClientDashboardDto
{
    public required IReadOnlyList<AssignedCaregiverDto> AssignedCaregivers { get; init; }

    public VisitSummaryDto? NextVisit { get; init; }

    public required IReadOnlyList<VisitSummaryDto> UpcomingVisits { get; init; }

    public required int CompletedVisitCount { get; init; }

    public required IReadOnlyList<VisitNoteDto> RecentVisitNotes { get; init; }
}
