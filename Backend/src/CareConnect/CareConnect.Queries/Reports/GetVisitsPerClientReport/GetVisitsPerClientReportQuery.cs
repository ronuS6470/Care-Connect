using CareConnect.DTOs.Reports;
using MediatR;

namespace CareConnect.Queries.Reports.GetVisitsPerClientReport;

public sealed record GetVisitsPerClientReportQuery(DateOnly FromDate, DateOnly ToDate, string RequestingAuth0UserId)
    : IRequest<IReadOnlyList<VisitsPerClientReportRowDto>>;
