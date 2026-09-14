using CareConnect.Infrastructure.Persistence;
using MediatR;

namespace CareConnect.Commands.Visits;

public sealed class UpdateVisitTaskCommandHandler : IRequestHandler<UpdateVisitTaskCommand>
{
    private readonly CareConnectDbContext _dbContext;

    public UpdateVisitTaskCommandHandler(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(UpdateVisitTaskCommand request, CancellationToken cancellationToken)
    {
        var (visit, task) = await VisitTaskLookup.LoadAsync(_dbContext, request.VisitId, request.VisitTaskId, cancellationToken);

        await VisitAccessControl.EnsureCanManageVisitAsync(_dbContext, visit, request.RequestingAuth0UserId, cancellationToken);
        VisitTaskLookup.EnsureVisitIsEditable(visit);

        task.Notes = request.Task.Notes;
        task.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
