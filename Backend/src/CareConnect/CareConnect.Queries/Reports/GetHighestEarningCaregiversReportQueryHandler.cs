using CareConnect.DTOs.Reports;
using CareConnect.Queries.Reports.Repositories;
using MediatR;

namespace CareConnect.Queries.Reports;

public sealed class GetHighestEarningCaregiversReportQueryHandler
    : IRequestHandler<GetHighestEarningCaregiversReportQuery, IReadOnlyList<HighestEarningCaregiverReportRowDto>>
{
    private readonly IReportsReadRepository _repository;

    public GetHighestEarningCaregiversReportQueryHandler(IReportsReadRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<HighestEarningCaregiverReportRowDto>> Handle(
        GetHighestEarningCaregiversReportQuery request, CancellationToken cancellationToken) =>
        _repository.GetHighestEarningCaregiversAsync(request.FromDate, request.ToDate, request.TopN, request.RequestingAuth0UserId, cancellationToken);
}
