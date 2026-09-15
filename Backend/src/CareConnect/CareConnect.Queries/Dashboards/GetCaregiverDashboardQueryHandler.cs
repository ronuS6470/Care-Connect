using CareConnect.DTOs.Dashboards;
using CareConnect.Queries.Dashboards.Repositories;
using MediatR;

namespace CareConnect.Queries.Dashboards;

public sealed class GetCaregiverDashboardQueryHandler : IRequestHandler<GetCaregiverDashboardQuery, CaregiverDashboardDto>
{
    private readonly IDashboardReadRepository _repository;

    public GetCaregiverDashboardQueryHandler(IDashboardReadRepository repository)
    {
        _repository = repository;
    }

    public Task<CaregiverDashboardDto> Handle(GetCaregiverDashboardQuery request, CancellationToken cancellationToken) =>
        _repository.GetCaregiverDashboardAsync(request.RequestingAuth0UserId, cancellationToken);
}
