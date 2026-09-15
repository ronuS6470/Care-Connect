using CareConnect.DTOs.Reports;
using CareConnect.Queries.Reports.Repositories;
using MediatR;

namespace CareConnect.Queries.Reports;

public sealed class GetVisitsPerCaregiverReportQueryHandler
    : IRequestHandler<GetVisitsPerCaregiverReportQuery, IReadOnlyList<VisitsPerCaregiverReportRowDto>>
{
    private readonly IReportsReadRepository _repository;

    public GetVisitsPerCaregiverReportQueryHandler(IReportsReadRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<VisitsPerCaregiverReportRowDto>> Handle(GetVisitsPerCaregiverReportQuery request, CancellationToken cancellationToken) =>
        _repository.GetVisitsPerCaregiverAsync(request.FromDate, request.ToDate, request.RequestingAuth0UserId, cancellationToken);
}
