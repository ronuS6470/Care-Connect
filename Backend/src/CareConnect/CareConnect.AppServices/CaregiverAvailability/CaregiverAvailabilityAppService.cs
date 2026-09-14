using CareConnect.Commands.Availability;
using CareConnect.DTOs.Caregivers;
using CareConnect.Queries.Availability;
using MediatR;

namespace CareConnect.AppServices.CaregiverAvailability;

public sealed class CaregiverAvailabilityAppService : ICaregiverAvailabilityAppService
{
    private readonly IMediator _mediator;

    public CaregiverAvailabilityAppService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<IReadOnlyList<CaregiverAvailabilityDto>> GetCaregiverAvailabilityAsync(int caregiverId, CancellationToken cancellationToken) =>
        _mediator.Send(new GetCaregiverAvailabilityQuery(caregiverId), cancellationToken);

    public Task<int> CreateAvailabilityAsync(CreateCaregiverAvailabilityDto dto, CancellationToken cancellationToken) =>
        _mediator.Send(new CreateAvailabilityCommand(dto), cancellationToken);

    public Task UpdateAvailabilityAsync(int availabilityId, UpdateCaregiverAvailabilityDto dto, CancellationToken cancellationToken) =>
        _mediator.Send(new UpdateAvailabilityCommand(availabilityId, dto), cancellationToken);

    public Task DeleteAvailabilityAsync(int availabilityId, CancellationToken cancellationToken) =>
        _mediator.Send(new DeleteAvailabilityCommand(availabilityId), cancellationToken);
}
