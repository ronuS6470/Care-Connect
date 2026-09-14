using CareConnect.Infrastructure.Errors;
using CareConnect.Infrastructure.Repositories.Clients;
using MediatR;

namespace CareConnect.Commands.Clients;

public sealed class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand>
{
    private readonly IClientRepository _repository;

    public DeleteClientCommandHandler(IClientRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        var client = await _repository.GetByIdAsync(request.ClientId, cancellationToken)
            ?? throw new NotFoundException($"Client {request.ClientId} was not found.");

        client.IsActive = false;
        client.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.SaveChangesAsync(cancellationToken);
    }
}
