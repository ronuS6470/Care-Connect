using CareConnect.Commands.Caregivers;
using CareConnect.DTOs.Caregivers;
using CareConnect.DTOs.Common;
using CareConnect.Queries.Caregivers;
using MediatR;

namespace CareConnect.AppServices.Caregivers;

public sealed class CaregiversAppService : ICaregiversAppService
{
    private readonly IMediator _mediator;

    public CaregiversAppService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<PagedResponseDto<CaregiverDto>> GetCaregiversAsync(int page, int pageSize, CancellationToken cancellationToken) =>
        _mediator.Send(new GetCaregiversQuery(page, pageSize), cancellationToken);

    public Task<CaregiverDto?> GetCaregiverByIdAsync(int caregiverId, CancellationToken cancellationToken) =>
        _mediator.Send(new GetCaregiverByIdQuery(caregiverId), cancellationToken);

    public Task<int> CreateCaregiverAsync(CreateCaregiverDto dto, CancellationToken cancellationToken) =>
        _mediator.Send(new CreateCaregiverCommand(dto), cancellationToken);

    public Task UpdateCaregiverAsync(int caregiverId, UpdateCaregiverDto dto, CancellationToken cancellationToken) =>
        _mediator.Send(new UpdateCaregiverCommand(caregiverId, dto), cancellationToken);

    public Task DeleteCaregiverAsync(int caregiverId, CancellationToken cancellationToken) =>
        _mediator.Send(new DeleteCaregiverCommand(caregiverId), cancellationToken);

    public Task ActivateCaregiverAsync(int caregiverId, CancellationToken cancellationToken) =>
        _mediator.Send(new ActivateCaregiverCommand(caregiverId), cancellationToken);

    public Task DeactivateCaregiverAsync(int caregiverId, CancellationToken cancellationToken) =>
        _mediator.Send(new DeactivateCaregiverCommand(caregiverId), cancellationToken);
}
