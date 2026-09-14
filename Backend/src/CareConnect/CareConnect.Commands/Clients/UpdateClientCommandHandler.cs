using AutoMapper;
using CareConnect.Infrastructure.Errors;
using CareConnect.Infrastructure.Repositories.Clients;
using MediatR;

namespace CareConnect.Commands.Clients;

public sealed class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand>
{
    private readonly IClientRepository _repository;
    private readonly IMapper _mapper;

    public UpdateClientCommandHandler(IClientRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        var client = await _repository.GetByIdAsync(request.ClientId, cancellationToken)
            ?? throw new NotFoundException($"Client {request.ClientId} was not found.");

        _mapper.Map(request.Client, client);

        await _repository.SaveChangesAsync(cancellationToken);
    }
}
