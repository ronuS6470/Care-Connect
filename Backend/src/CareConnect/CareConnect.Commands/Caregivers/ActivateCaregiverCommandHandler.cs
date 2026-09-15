using CareConnect.Infrastructure.Repositories.Caregivers;
using MediatR;

namespace CareConnect.Commands.Caregivers;

public sealed class ActivateCaregiverCommandHandler : IRequestHandler<ActivateCaregiverCommand>
{
    private readonly ICaregiverRepository _repository;

    public ActivateCaregiverCommandHandler(ICaregiverRepository repository)
    {
        _repository = repository;
    }

    public Task Handle(ActivateCaregiverCommand request, CancellationToken cancellationToken) =>
        _repository.SetActiveStatusAsync(request.CaregiverId, isActive: true, cancellationToken);
}
