using CareConnect.DTOs.Reports;
using MediatR;

namespace CareConnect.Queries.Reports;

public sealed record GetTopCaregiversByHoursReportQuery(DateOnly FromDate, DateOnly ToDate, int TopN, string RequestingAuth0UserId)
    : IRequest<IReadOnlyList<TopCaregiverByHoursReportRowDto>>;
