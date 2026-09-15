using CareConnect.DTOs.Reporting;
using MediatR;

namespace CareConnect.Queries.Reports;

public sealed record GetTodaysVisitsReportQuery(string RequestingAuth0UserId) : IRequest<IReadOnlyList<VisitSummaryDto>>;
