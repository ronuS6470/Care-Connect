using CareConnect.DTOs.Reporting;
using CareConnect.Queries.Reports.Repositories;
using MediatR;

namespace CareConnect.Queries.Reports;

public sealed class GetCompletedVisitsReportQueryHandler : IRequestHandler<GetCompletedVisitsReportQuery, IReadOnlyList<VisitSummaryDto>>
{
    private readonly IReportsReadRepository _repository;

    public GetCompletedVisitsReportQueryHandler(IReportsReadRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<VisitSummaryDto>> Handle(GetCompletedVisitsReportQuery request, CancellationToken cancellationToken) =>
        _repository.GetCompletedVisitsAsync(request.FromDate, request.ToDate, request.RequestingAuth0UserId, cancellationToken);
}
