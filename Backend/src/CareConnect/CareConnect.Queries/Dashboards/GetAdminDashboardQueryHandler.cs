using CareConnect.DTOs.Dashboards;
using CareConnect.Queries.Dashboards.Repositories;
using MediatR;

namespace CareConnect.Queries.Dashboards;

public sealed class GetAdminDashboardQueryHandler : IRequestHandler<GetAdminDashboardQuery, AdminDashboardDto>
{
    private readonly IDashboardReadRepository _repository;

    public GetAdminDashboardQueryHandler(IDashboardReadRepository repository)
    {
        _repository = repository;
    }

    public Task<AdminDashboardDto> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken) =>
        _repository.GetAdminDashboardAsync(request.RequestingAuth0UserId, cancellationToken);
}
