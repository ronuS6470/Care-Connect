using CareConnect.DTOs.Reports;
using CareConnect.Queries.Reports.Repositories;
using MediatR;

namespace CareConnect.Queries.Reports;

public sealed class GetTopCaregiversByHoursReportQueryHandler
    : IRequestHandler<GetTopCaregiversByHoursReportQuery, IReadOnlyList<TopCaregiverByHoursReportRowDto>>
{
    private readonly IReportsReadRepository _repository;

    public GetTopCaregiversByHoursReportQueryHandler(IReportsReadRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<TopCaregiverByHoursReportRowDto>> Handle(GetTopCaregiversByHoursReportQuery request, CancellationToken cancellationToken) =>
        _repository.GetTopCaregiversByHoursAsync(request.FromDate, request.ToDate, request.TopN, request.RequestingAuth0UserId, cancellationToken);
}
