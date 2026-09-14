using CareConnect.AppServices.Caregivers;
using MediatR;

namespace CareConnect.Commands.Caregivers;

public sealed class DeleteCaregiverCommandHandler : IRequestHandler<DeleteCaregiverCommand>
{
    private readonly ICaregiverActivationService _activationService;

    public DeleteCaregiverCommandHandler(ICaregiverActivationService activationService)
    {
        _activationService = activationService;
    }

    public Task Handle(DeleteCaregiverCommand request, CancellationToken cancellationToken) =>
        _activationService.SetActiveStatusAsync(request.CaregiverId, isActive: false, cancellationToken);
}
