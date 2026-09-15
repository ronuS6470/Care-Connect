using CareConnect.DTOs.Common;
using CareConnect.DTOs.Reporting;
using MediatR;

namespace CareConnect.Queries.Visits.GetUpcomingVisits;

public sealed record GetUpcomingVisitsQuery(int Page, int PageSize, string RequestingAuth0UserId)
    : IRequest<PagedResponseDto<VisitSummaryDto>>;
