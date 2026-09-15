using CareConnect.Infrastructure.Repositories.Caregivers;
using MediatR;

namespace CareConnect.Commands.Caregivers;

public sealed class DeleteCaregiverCommandHandler : IRequestHandler<DeleteCaregiverCommand>
{
    private readonly ICaregiverRepository _repository;

    public DeleteCaregiverCommandHandler(ICaregiverRepository repository)
    {
        _repository = repository;
    }

    public Task Handle(DeleteCaregiverCommand request, CancellationToken cancellationToken) =>
        _repository.SetActiveStatusAsync(request.CaregiverId, isActive: false, cancellationToken);
}
