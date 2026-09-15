using CareConnect.DTOs.Reports;
using CareConnect.Queries.Reports.Repositories;
using MediatR;

namespace CareConnect.Queries.Reports;

public sealed class GetCaregiverAvailabilityConflictsReportQueryHandler
    : IRequestHandler<GetCaregiverAvailabilityConflictsReportQuery, IReadOnlyList<CaregiverAvailabilityConflictDto>>
{
    private readonly IReportsReadRepository _repository;

    public GetCaregiverAvailabilityConflictsReportQueryHandler(IReportsReadRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<CaregiverAvailabilityConflictDto>> Handle(
        GetCaregiverAvailabilityConflictsReportQuery request, CancellationToken cancellationToken) =>
        _repository.GetCaregiverAvailabilityConflictsAsync(request.RequestingAuth0UserId, cancellationToken);
}
