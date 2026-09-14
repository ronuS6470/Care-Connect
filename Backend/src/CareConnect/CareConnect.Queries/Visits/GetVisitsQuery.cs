using CareConnect.DTOs.Common;
using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Reporting;
using MediatR;

namespace CareConnect.Queries.Visits;

public sealed record GetVisitsQuery(
    int Page,
    int PageSize,
    DateOnly? FromDate,
    DateOnly? ToDate,
    int? CaregiverId,
    int? ClientId,
    VisitStatus? Status,
    string? Search,
    string RequestingAuth0UserId) : IRequest<PagedResponseDto<VisitSummaryDto>>;
