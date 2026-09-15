using CareConnect.DTOs.Common;
using CareConnect.DTOs.Reporting;
using CareConnect.Queries.Visits.Repositories;
using MediatR;

namespace CareConnect.Queries.Visits;

public sealed class GetUpcomingVisitsQueryHandler : IRequestHandler<GetUpcomingVisitsQuery, PagedResponseDto<VisitSummaryDto>>
{
    private readonly IVisitReadRepository _repository;

    public GetUpcomingVisitsQueryHandler(IVisitReadRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResponseDto<VisitSummaryDto>> Handle(GetUpcomingVisitsQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        return _repository.GetUpcomingAsync(page, pageSize, request.RequestingAuth0UserId, cancellationToken);
    }
}
