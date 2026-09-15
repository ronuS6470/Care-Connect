using CareConnect.DTOs.Reports;
using MediatR;

namespace CareConnect.Queries.Reports.GetCaregiverTotalHoursReport;

public sealed record GetCaregiverTotalHoursReportQuery(DateOnly FromDate, DateOnly ToDate, string RequestingAuth0UserId)
    : IRequest<IReadOnlyList<CaregiverHoursReportRowDto>>;
