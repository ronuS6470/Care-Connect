using CareConnect.DTOs.Reports;
using MediatR;

namespace CareConnect.Queries.Reports.GetCaregiverAvailabilityConflictsReport;

public sealed record GetCaregiverAvailabilityConflictsReportQuery(string RequestingAuth0UserId)
    : IRequest<IReadOnlyList<CaregiverAvailabilityConflictDto>>;
