using CareConnect.DTOs.Reports;
using MediatR;

namespace CareConnect.Queries.Reports;

public sealed record GetCaregiverAvailabilityConflictsReportQuery(string RequestingAuth0UserId)
    : IRequest<IReadOnlyList<CaregiverAvailabilityConflictDto>>;
