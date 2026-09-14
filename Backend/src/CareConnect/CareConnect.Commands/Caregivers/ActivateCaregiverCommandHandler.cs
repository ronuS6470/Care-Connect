using CareConnect.AppServices.Caregivers;
using MediatR;

namespace CareConnect.Commands.Caregivers;

public sealed class ActivateCaregiverCommandHandler : IRequestHandler<ActivateCaregiverCommand>
{
    private readonly ICaregiverActivationService _activationService;

    public ActivateCaregiverCommandHandler(ICaregiverActivationService activationService)
    {
        _activationService = activationService;
    }

    public Task Handle(ActivateCaregiverCommand request, CancellationToken cancellationToken) =>
        _activationService.SetActiveStatusAsync(request.CaregiverId, isActive: true, cancellationToken);
}
