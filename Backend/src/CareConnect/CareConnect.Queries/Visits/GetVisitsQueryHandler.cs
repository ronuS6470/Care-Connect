using CareConnect.DTOs.Common;
using CareConnect.DTOs.Reporting;
using CareConnect.Queries.Visits.Repositories;
using MediatR;

namespace CareConnect.Queries.Visits;

public sealed class GetVisitsQueryHandler : IRequestHandler<GetVisitsQuery, PagedResponseDto<VisitSummaryDto>>
{
    private readonly IVisitReadRepository _repository;

    public GetVisitsQueryHandler(IVisitReadRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResponseDto<VisitSummaryDto>> Handle(GetVisitsQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        return _repository.GetPagedAsync(
            page, pageSize, request.FromDate, request.ToDate, request.CaregiverId, request.ClientId,
            request.Status, request.Search, request.RequestingAuth0UserId, cancellationToken);
    }
}
