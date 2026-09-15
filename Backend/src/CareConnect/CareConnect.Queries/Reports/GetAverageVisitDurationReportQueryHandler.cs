using CareConnect.DTOs.Reports;
using CareConnect.Queries.Reports.Repositories;
using MediatR;

namespace CareConnect.Queries.Reports;

public sealed class GetAverageVisitDurationReportQueryHandler
    : IRequestHandler<GetAverageVisitDurationReportQuery, IReadOnlyList<AverageVisitDurationReportRowDto>>
{
    private readonly IReportsReadRepository _repository;

    public GetAverageVisitDurationReportQueryHandler(IReportsReadRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<AverageVisitDurationReportRowDto>> Handle(GetAverageVisitDurationReportQuery request, CancellationToken cancellationToken) =>
        _repository.GetAverageVisitDurationAsync(request.FromDate, request.ToDate, request.RequestingAuth0UserId, cancellationToken);
}
