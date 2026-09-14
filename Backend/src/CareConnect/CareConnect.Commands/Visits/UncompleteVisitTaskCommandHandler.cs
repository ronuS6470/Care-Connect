using CareConnect.DTOs.Errors;
using CareConnect.Infrastructure.Persistence;
using MediatR;

namespace CareConnect.Commands.Visits;

public sealed class UncompleteVisitTaskCommandHandler : IRequestHandler<UncompleteVisitTaskCommand>
{
    private readonly CareConnectDbContext _dbContext;

    public UncompleteVisitTaskCommandHandler(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(UncompleteVisitTaskCommand request, CancellationToken cancellationToken)
    {
        var (visit, task) = await VisitTaskLookup.LoadAsync(_dbContext, request.VisitId, request.VisitTaskId, cancellationToken);

        await VisitAccessControl.EnsureCanManageVisitAsync(_dbContext, visit, request.RequestingAuth0UserId, cancellationToken);

        // The business rule gating this specific command: once a visit is closed out
        // (Completed/Cancelled/NoShow), its task record is history and can't be reopened.
        VisitTaskLookup.EnsureVisitIsEditable(visit);

        if (!task.IsCompleted)
        {
            throw new BusinessRuleViolationException("This task is not marked complete.");
        }

        task.IsCompleted = false;
        task.CompletedAtUtc = null;
        task.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
