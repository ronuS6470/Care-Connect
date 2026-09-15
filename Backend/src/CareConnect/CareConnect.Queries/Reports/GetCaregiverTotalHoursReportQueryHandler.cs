using CareConnect.DTOs.Reports;
using CareConnect.Queries.Reports.Repositories;
using MediatR;

namespace CareConnect.Queries.Reports;

public sealed class GetCaregiverTotalHoursReportQueryHandler
    : IRequestHandler<GetCaregiverTotalHoursReportQuery, IReadOnlyList<CaregiverHoursReportRowDto>>
{
    private readonly IReportsReadRepository _repository;

    public GetCaregiverTotalHoursReportQueryHandler(IReportsReadRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<CaregiverHoursReportRowDto>> Handle(GetCaregiverTotalHoursReportQuery request, CancellationToken cancellationToken) =>
        _repository.GetCaregiverTotalHoursAsync(request.FromDate, request.ToDate, request.RequestingAuth0UserId, cancellationToken);
}
