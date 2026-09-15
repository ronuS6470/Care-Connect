using CareConnect.AppServices.Security;
using CareConnect.Commands.Clients;
using CareConnect.DTOs.Clients;
using CareConnect.DTOs.Common;
using CareConnect.Queries.Clients.GetClientById;
using CareConnect.Queries.Clients.GetClients;
using MediatR;

namespace CareConnect.AppServices.Clients;

public sealed class ClientsAppService : IClientsAppService
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public ClientsAppService(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    {
        _mediator = mediator;
        _currentUserAccessor = currentUserAccessor;
    }

    public Task<PagedResponseDto<ClientDto>> GetClientsAsync(
        int page, int pageSize, string? search, bool? isActive, CancellationToken cancellationToken) =>
        _mediator.Send(new GetClientsQuery(page, pageSize, search, isActive, _currentUserAccessor.Auth0UserId), cancellationToken);

    public Task<ClientDto?> GetClientByIdAsync(int clientId, CancellationToken cancellationToken) =>
        _mediator.Send(new GetClientByIdQuery(clientId, _currentUserAccessor.Auth0UserId), cancellationToken);

    public Task<int> CreateClientAsync(CreateClientDto dto, CancellationToken cancellationToken) =>
        _mediator.Send(new CreateClientCommand(dto), cancellationToken);

    public Task UpdateClientAsync(int clientId, UpdateClientDto dto, CancellationToken cancellationToken) =>
        _mediator.Send(new UpdateClientCommand(clientId, dto), cancellationToken);

    public Task DeleteClientAsync(int clientId, CancellationToken cancellationToken) =>
        _mediator.Send(new DeleteClientCommand(clientId), cancellationToken);
}
