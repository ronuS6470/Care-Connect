using CareConnect.Infrastructure.Errors;
using CareConnect.Infrastructure.Repositories.CareTasks;
using MediatR;

namespace CareConnect.Commands.CareTasks;

public sealed class DeleteCareTaskCommandHandler : IRequestHandler<DeleteCareTaskCommand>
{
    private readonly ICareTaskRepository _repository;

    public DeleteCareTaskCommandHandler(ICareTaskRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeleteCareTaskCommand request, CancellationToken cancellationToken)
    {
        var careTask = await _repository.GetByIdAsync(request.CareTaskId, cancellationToken)
            ?? throw new NotFoundException($"Care task {request.CareTaskId} was not found.");

        var isReferencedByVisitTasks = await _repository.IsReferencedByVisitTaskAsync(request.CareTaskId, cancellationToken);

        if (isReferencedByVisitTasks)
        {
            // Historical visit records point at this task — deactivate instead of breaking them.
            careTask.IsActive = false;
            careTask.UpdatedAtUtc = DateTime.UtcNow;
        }
        else
        {
            _repository.Remove(careTask);
        }

        await _repository.SaveChangesAsync(cancellationToken);
    }
}
