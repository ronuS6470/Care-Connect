using CareConnect.DTOs.Dashboards;

namespace CareConnect.Queries.Dashboards.Repositories;

public interface IDashboardReadRepository
{
    Task<AdminDashboardDto> GetAdminDashboardAsync(string requestingAuth0UserId, CancellationToken cancellationToken);

    Task<CaregiverDashboardDto> GetCaregiverDashboardAsync(string requestingAuth0UserId, CancellationToken cancellationToken);

    Task<ClientDashboardDto> GetClientDashboardAsync(string requestingAuth0UserId, CancellationToken cancellationToken);
}
