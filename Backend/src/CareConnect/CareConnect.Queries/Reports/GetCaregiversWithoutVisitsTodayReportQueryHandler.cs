using CareConnect.DTOs.Reports;
using CareConnect.Queries.Reports.Repositories;
using MediatR;

namespace CareConnect.Queries.Reports;

public sealed class GetCaregiversWithoutVisitsTodayReportQueryHandler
    : IRequestHandler<GetCaregiversWithoutVisitsTodayReportQuery, IReadOnlyList<CaregiverWithoutVisitsTodayDto>>
{
    private readonly IReportsReadRepository _repository;

    public GetCaregiversWithoutVisitsTodayReportQueryHandler(IReportsReadRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<CaregiverWithoutVisitsTodayDto>> Handle(
        GetCaregiversWithoutVisitsTodayReportQuery request, CancellationToken cancellationToken) =>
        _repository.GetCaregiversWithoutVisitsTodayAsync(request.RequestingAuth0UserId, cancellationToken);
}
