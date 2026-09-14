using CareConnect.DTOs.Errors;
using CareConnect.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Commands.CareTasks;

public sealed class UpdateCareTaskCommandHandler : IRequestHandler<UpdateCareTaskCommand>
{
    private readonly CareConnectDbContext _dbContext;

    public UpdateCareTaskCommandHandler(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(UpdateCareTaskCommand request, CancellationToken cancellationToken)
    {
        var careTask = await _dbContext.CareTasks
            .FirstOrDefaultAsync(t => t.Id == request.CareTaskId, cancellationToken)
            ?? throw new NotFoundException($"Care task {request.CareTaskId} was not found.");

        var dto = request.CareTask;

        var nameUsedByAnotherTask = await _dbContext.CareTasks
            .AnyAsync(t => t.Id != request.CareTaskId && t.Name == dto.Name, cancellationToken);

        if (nameUsedByAnotherTask)
        {
            throw new BusinessRuleViolationException($"A care task named '{dto.Name}' already exists.");
        }

        careTask.Name = dto.Name;
        careTask.Description = dto.Description;
        careTask.IsActive = dto.IsActive;
        careTask.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
