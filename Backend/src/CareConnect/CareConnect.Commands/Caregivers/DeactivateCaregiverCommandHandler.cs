using CareConnect.Infrastructure.Repositories.Caregivers;
using MediatR;

namespace CareConnect.Commands.Caregivers;

public sealed class DeactivateCaregiverCommandHandler : IRequestHandler<DeactivateCaregiverCommand>
{
    private readonly ICaregiverRepository _repository;

    public DeactivateCaregiverCommandHandler(ICaregiverRepository repository)
    {
        _repository = repository;
    }

    public Task Handle(DeactivateCaregiverCommand request, CancellationToken cancellationToken) =>
        _repository.SetActiveStatusAsync(request.CaregiverId, isActive: false, cancellationToken);
}
