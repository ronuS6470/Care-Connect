using CareConnect.DTOs.Dashboards;

namespace CareConnect.AppServices.Dashboards;

public interface IDashboardAppService
{
    Task<AdminDashboardDto> GetAdminDashboardAsync(CancellationToken cancellationToken);

    Task<CaregiverDashboardDto> GetCaregiverDashboardAsync(CancellationToken cancellationToken);

    Task<ClientDashboardDto> GetClientDashboardAsync(CancellationToken cancellationToken);
}
