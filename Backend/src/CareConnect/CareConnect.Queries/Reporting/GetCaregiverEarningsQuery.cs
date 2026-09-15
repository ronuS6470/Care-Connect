using CareConnect.DTOs.Reporting;
using MediatR;

namespace CareConnect.Queries.Reporting;

/// <summary>
/// Earnings for one caregiver over [FromDate, ToDate], computed from completed visits only using
/// actual check-in/check-out time — never the scheduled window.
/// </summary>
public sealed record GetCaregiverEarningsQuery(
    int CaregiverId,
    DateOnly FromDate,
    DateOnly ToDate,
    bool IncludeVisitBreakdown,
    string RequestingAuth0UserId) : IRequest<CaregiverEarningsDto>;
