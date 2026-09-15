using CareConnect.DTOs.Reports;
using MediatR;

namespace CareConnect.Queries.Reports;

public sealed record GetHighestEarningCaregiversReportQuery(DateOnly FromDate, DateOnly ToDate, int TopN, string RequestingAuth0UserId)
    : IRequest<IReadOnlyList<HighestEarningCaregiverReportRowDto>>;
