using CareConnect.DTOs.Reporting;
using CareConnect.Queries.Reports.Repositories;
using MediatR;

namespace CareConnect.Queries.Reports;

public sealed class GetTodaysVisitsReportQueryHandler : IRequestHandler<GetTodaysVisitsReportQuery, IReadOnlyList<VisitSummaryDto>>
{
    private readonly IReportsReadRepository _repository;

    public GetTodaysVisitsReportQueryHandler(IReportsReadRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<VisitSummaryDto>> Handle(GetTodaysVisitsReportQuery request, CancellationToken cancellationToken) =>
        _repository.GetTodaysVisitsAsync(request.RequestingAuth0UserId, cancellationToken);
}
