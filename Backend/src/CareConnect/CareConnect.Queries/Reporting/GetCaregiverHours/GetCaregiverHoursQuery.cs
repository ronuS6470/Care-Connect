using CareConnect.DTOs.Reporting;
using MediatR;

namespace CareConnect.Queries.Reporting.GetCaregiverHours;

/// <summary>Hours worked per day for one caregiver over [FromDate, ToDate], completed visits only.</summary>
public sealed record GetCaregiverHoursQuery(
    int CaregiverId,
    DateOnly FromDate,
    DateOnly ToDate,
    string RequestingAuth0UserId) : IRequest<IReadOnlyList<WorkedHoursDto>>;
