using CareConnect.DTOs.Reports;
using MediatR;

namespace CareConnect.Queries.Reports.GetTopCaregiversByCompletedVisitsReport;

public sealed record GetTopCaregiversByCompletedVisitsReportQuery(DateOnly FromDate, DateOnly ToDate, int TopN, string RequestingAuth0UserId)
    : IRequest<IReadOnlyList<TopCompletedVisitsCaregiverReportRowDto>>;
