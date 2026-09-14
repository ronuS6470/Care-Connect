using CareConnect.DTOs.Clients;
using CareConnect.Queries.Clients.Repositories;
using MediatR;

namespace CareConnect.Queries.Clients;

public sealed class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, ClientDto?>
{
    private readonly IClientReadRepository _repository;

    public GetClientByIdQueryHandler(IClientReadRepository repository)
    {
        _repository = repository;
    }

    public Task<ClientDto?> Handle(GetClientByIdQuery request, CancellationToken cancellationToken) =>
        _repository.GetByIdAsync(request.ClientId, request.RequestingAuth0UserId, cancellationToken);
}
