using CareConnect.DTOs.Reports;
using MediatR;

namespace CareConnect.Queries.Reports.GetCaregiverTotalEarningsReport;

public sealed record GetCaregiverTotalEarningsReportQuery(DateOnly FromDate, DateOnly ToDate, string RequestingAuth0UserId)
    : IRequest<IReadOnlyList<CaregiverEarningsReportRowDto>>;
