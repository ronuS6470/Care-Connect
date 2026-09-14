using CareConnect.DTOs.Clients;
using CareConnect.DTOs.Common;

namespace CareConnect.Queries.Clients.Repositories;

public interface IClientReadRepository
{
    Task<PagedResponseDto<ClientDto>> GetPagedAsync(
        int page, int pageSize, string? search, bool? isActive, string requestingAuth0UserId, CancellationToken cancellationToken);

    Task<ClientDto?> GetByIdAsync(int clientId, string requestingAuth0UserId, CancellationToken cancellationToken);
}
