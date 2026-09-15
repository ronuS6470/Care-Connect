using CareConnect.DTOs.Reports;
using CareConnect.Queries.Reports.Repositories;
using MediatR;

namespace CareConnect.Queries.Reports;

public sealed class GetTopCaregiversByCompletedVisitsReportQueryHandler
    : IRequestHandler<GetTopCaregiversByCompletedVisitsReportQuery, IReadOnlyList<TopCompletedVisitsCaregiverReportRowDto>>
{
    private readonly IReportsReadRepository _repository;

    public GetTopCaregiversByCompletedVisitsReportQueryHandler(IReportsReadRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<TopCompletedVisitsCaregiverReportRowDto>> Handle(
        GetTopCaregiversByCompletedVisitsReportQuery request, CancellationToken cancellationToken) =>
        _repository.GetTopCaregiversByCompletedVisitsAsync(
            request.FromDate, request.ToDate, request.TopN, request.RequestingAuth0UserId, cancellationToken);
}
