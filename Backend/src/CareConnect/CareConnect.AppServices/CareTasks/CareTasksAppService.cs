using CareConnect.Commands.CareTasks;
using CareConnect.DTOs.CareTasks;
using CareConnect.DTOs.Common;
using CareConnect.Queries.CareTasks;
using MediatR;

namespace CareConnect.AppServices.CareTasks;

public sealed class CareTasksAppService : ICareTasksAppService
{
    private readonly IMediator _mediator;

    public CareTasksAppService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<PagedResponseDto<CareTaskDto>> GetCareTasksAsync(int page, int pageSize, bool? isActive, CancellationToken cancellationToken) =>
        _mediator.Send(new GetCareTasksQuery(page, pageSize, isActive), cancellationToken);

    public Task<CareTaskDto?> GetCareTaskByIdAsync(int careTaskId, CancellationToken cancellationToken) =>
        _mediator.Send(new GetCareTaskByIdQuery(careTaskId), cancellationToken);

    public Task<int> CreateCareTaskAsync(CreateCareTaskDto dto, CancellationToken cancellationToken) =>
        _mediator.Send(new CreateCareTaskCommand(dto), cancellationToken);

    public Task UpdateCareTaskAsync(int careTaskId, UpdateCareTaskDto dto, CancellationToken cancellationToken) =>
        _mediator.Send(new UpdateCareTaskCommand(careTaskId, dto), cancellationToken);

    public Task DeleteCareTaskAsync(int careTaskId, CancellationToken cancellationToken) =>
        _mediator.Send(new DeleteCareTaskCommand(careTaskId), cancellationToken);
}
