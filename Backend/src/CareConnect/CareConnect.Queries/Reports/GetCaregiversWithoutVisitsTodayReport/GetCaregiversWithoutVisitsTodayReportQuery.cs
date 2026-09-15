using CareConnect.DTOs.Reports;
using MediatR;

namespace CareConnect.Queries.Reports.GetCaregiversWithoutVisitsTodayReport;

public sealed record GetCaregiversWithoutVisitsTodayReportQuery(string RequestingAuth0UserId)
    : IRequest<IReadOnlyList<CaregiverWithoutVisitsTodayDto>>;
