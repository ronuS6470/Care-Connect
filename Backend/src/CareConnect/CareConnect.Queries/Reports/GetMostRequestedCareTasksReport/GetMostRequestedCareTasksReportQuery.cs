using CareConnect.DTOs.Reports;
using MediatR;

namespace CareConnect.Queries.Reports.GetMostRequestedCareTasksReport;

public sealed record GetMostRequestedCareTasksReportQuery(DateOnly FromDate, DateOnly ToDate, string RequestingAuth0UserId)
    : IRequest<IReadOnlyList<CareTaskDemandReportRowDto>>;
