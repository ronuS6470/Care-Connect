using CareConnect.DTOs.Clients;
using CareConnect.DTOs.Common;

namespace CareConnect.AppServices.Clients;

public interface IClientsAppService
{
    Task<PagedResponseDto<ClientDto>> GetClientsAsync(
        int page, int pageSize, string? search, bool? isActive, CancellationToken cancellationToken);

    Task<ClientDto?> GetClientByIdAsync(int clientId, CancellationToken cancellationToken);

    Task<int> CreateClientAsync(CreateClientDto dto, CancellationToken cancellationToken);

    Task UpdateClientAsync(int clientId, UpdateClientDto dto, CancellationToken cancellationToken);

    Task DeleteClientAsync(int clientId, CancellationToken cancellationToken);
}
