using CareConnect.DTOs.Reports;
using CareConnect.Queries.Reports.Repositories;
using MediatR;

namespace CareConnect.Queries.Reports;

public sealed class GetMostRequestedCareTasksReportQueryHandler
    : IRequestHandler<GetMostRequestedCareTasksReportQuery, IReadOnlyList<CareTaskDemandReportRowDto>>
{
    private readonly IReportsReadRepository _repository;

    public GetMostRequestedCareTasksReportQueryHandler(IReportsReadRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<CareTaskDemandReportRowDto>> Handle(GetMostRequestedCareTasksReportQuery request, CancellationToken cancellationToken) =>
        _repository.GetMostRequestedCareTasksAsync(request.FromDate, request.ToDate, request.RequestingAuth0UserId, cancellationToken);
}
