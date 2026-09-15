using CareConnect.AppServices.Security;
using CareConnect.DTOs.Dashboards;
using CareConnect.Queries.Dashboards.GetAdminDashboard;
using CareConnect.Queries.Dashboards.GetCaregiverDashboard;
using CareConnect.Queries.Dashboards.GetClientDashboard;
using MediatR;

namespace CareConnect.AppServices.Dashboards;

public sealed class DashboardAppService : IDashboardAppService
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public DashboardAppService(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    {
        _mediator = mediator;
        _currentUserAccessor = currentUserAccessor;
    }

    public Task<AdminDashboardDto> GetAdminDashboardAsync(CancellationToken cancellationToken) =>
        _mediator.Send(new GetAdminDashboardQuery(_currentUserAccessor.Auth0UserId), cancellationToken);

    public Task<CaregiverDashboardDto> GetCaregiverDashboardAsync(CancellationToken cancellationToken) =>
        _mediator.Send(new GetCaregiverDashboardQuery(_currentUserAccessor.Auth0UserId), cancellationToken);

    public Task<ClientDashboardDto> GetClientDashboardAsync(CancellationToken cancellationToken) =>
        _mediator.Send(new GetClientDashboardQuery(_currentUserAccessor.Auth0UserId), cancellationToken);
}
