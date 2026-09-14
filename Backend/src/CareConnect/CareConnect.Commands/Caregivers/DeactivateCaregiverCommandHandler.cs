using CareConnect.AppServices.Caregivers;
using MediatR;

namespace CareConnect.Commands.Caregivers;

public sealed class DeactivateCaregiverCommandHandler : IRequestHandler<DeactivateCaregiverCommand>
{
    private readonly ICaregiverActivationService _activationService;

    public DeactivateCaregiverCommandHandler(ICaregiverActivationService activationService)
    {
        _activationService = activationService;
    }

    public Task Handle(DeactivateCaregiverCommand request, CancellationToken cancellationToken) =>
        _activationService.SetActiveStatusAsync(request.CaregiverId, isActive: false, cancellationToken);
}
