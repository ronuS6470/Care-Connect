using CareConnect.DTOs.Dashboards;
using CareConnect.Queries.Dashboards.Repositories;
using MediatR;

namespace CareConnect.Queries.Dashboards;

public sealed class GetClientDashboardQueryHandler : IRequestHandler<GetClientDashboardQuery, ClientDashboardDto>
{
    private readonly IDashboardReadRepository _repository;

    public GetClientDashboardQueryHandler(IDashboardReadRepository repository)
    {
        _repository = repository;
    }

    public Task<ClientDashboardDto> Handle(GetClientDashboardQuery request, CancellationToken cancellationToken) =>
        _repository.GetClientDashboardAsync(request.RequestingAuth0UserId, cancellationToken);
}
