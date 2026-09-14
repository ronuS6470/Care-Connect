using CareConnect.DTOs.Errors;
using CareConnect.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Commands.CareTasks;

public sealed class DeleteCareTaskCommandHandler : IRequestHandler<DeleteCareTaskCommand>
{
    private readonly CareConnectDbContext _dbContext;

    public DeleteCareTaskCommandHandler(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(DeleteCareTaskCommand request, CancellationToken cancellationToken)
    {
        var careTask = await _dbContext.CareTasks
            .FirstOrDefaultAsync(t => t.Id == request.CareTaskId, cancellationToken)
            ?? throw new NotFoundException($"Care task {request.CareTaskId} was not found.");

        var isReferencedByVisitTasks = await _dbContext.VisitTasks
            .AnyAsync(vt => vt.CareTaskId == request.CareTaskId, cancellationToken);

        if (isReferencedByVisitTasks)
        {
            // Historical visit records point at this task — deactivate instead of breaking them.
            careTask.IsActive = false;
            careTask.UpdatedAtUtc = DateTime.UtcNow;
        }
        else
        {
            _dbContext.CareTasks.Remove(careTask);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
