using CareConnect.DTOs.Errors;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Commands.CareTasks;

public sealed class CreateCareTaskCommandHandler : IRequestHandler<CreateCareTaskCommand, int>
{
    private readonly CareConnectDbContext _dbContext;

    public CreateCareTaskCommandHandler(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> Handle(CreateCareTaskCommand request, CancellationToken cancellationToken)
    {
        var dto = request.CareTask;

        var nameAlreadyExists = await _dbContext.CareTasks
            .AnyAsync(t => t.Name == dto.Name, cancellationToken);

        if (nameAlreadyExists)
        {
            throw new BusinessRuleViolationException($"A care task named '{dto.Name}' already exists.");
        }

        var careTask = new CareTask
        {
            Name = dto.Name,
            Description = dto.Description,
            IsActive = true,
        };

        _dbContext.CareTasks.Add(careTask);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return careTask.Id;
    }
}
