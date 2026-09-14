using CareConnect.DTOs.Errors;
using CareConnect.Infrastructure.Persistence;
using MediatR;

namespace CareConnect.Commands.Visits;

public sealed class CompleteVisitTaskCommandHandler : IRequestHandler<CompleteVisitTaskCommand>
{
    private readonly CareConnectDbContext _dbContext;

    public CompleteVisitTaskCommandHandler(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(CompleteVisitTaskCommand request, CancellationToken cancellationToken)
    {
        var (visit, task) = await VisitTaskLookup.LoadAsync(_dbContext, request.VisitId, request.VisitTaskId, cancellationToken);

        await VisitAccessControl.EnsureCanManageVisitAsync(_dbContext, visit, request.RequestingAuth0UserId, cancellationToken);
        VisitTaskLookup.EnsureVisitIsEditable(visit);

        if (task.IsCompleted)
        {
            throw new BusinessRuleViolationException("This task is already marked complete.");
        }

        var now = DateTime.UtcNow;
        task.IsCompleted = true;
        task.CompletedAtUtc = now;
        task.UpdatedAtUtc = now;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
