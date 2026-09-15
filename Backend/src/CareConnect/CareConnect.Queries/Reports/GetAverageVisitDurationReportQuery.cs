using CareConnect.DTOs.Reports;
using MediatR;

namespace CareConnect.Queries.Reports;

public sealed record GetAverageVisitDurationReportQuery(DateOnly FromDate, DateOnly ToDate, string RequestingAuth0UserId)
    : IRequest<IReadOnlyList<AverageVisitDurationReportRowDto>>;
