using CareConnect.DTOs.Reports;
using CareConnect.Queries.Reports.Repositories;
using MediatR;

namespace CareConnect.Queries.Reports;

public sealed class GetCaregiverTotalEarningsReportQueryHandler
    : IRequestHandler<GetCaregiverTotalEarningsReportQuery, IReadOnlyList<CaregiverEarningsReportRowDto>>
{
    private readonly IReportsReadRepository _repository;

    public GetCaregiverTotalEarningsReportQueryHandler(IReportsReadRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<CaregiverEarningsReportRowDto>> Handle(GetCaregiverTotalEarningsReportQuery request, CancellationToken cancellationToken) =>
        _repository.GetCaregiverTotalEarningsAsync(request.FromDate, request.ToDate, request.RequestingAuth0UserId, cancellationToken);
}
