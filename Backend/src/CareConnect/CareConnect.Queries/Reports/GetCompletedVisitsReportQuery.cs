using CareConnect.DTOs.Reporting;
using MediatR;

namespace CareConnect.Queries.Reports;

public sealed record GetCompletedVisitsReportQuery(DateOnly FromDate, DateOnly ToDate, string RequestingAuth0UserId)
    : IRequest<IReadOnlyList<VisitSummaryDto>>;
