using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Errors;
using CareConnect.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Commands.Visits;

public sealed class CancelVisitCommandHandler : IRequestHandler<CancelVisitCommand>
{
    private readonly CareConnectDbContext _dbContext;

    public CancelVisitCommandHandler(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(CancelVisitCommand request, CancellationToken cancellationToken)
    {
        var visit = await _dbContext.Visits.FirstOrDefaultAsync(v => v.Id == request.VisitId, cancellationToken)
            ?? throw new NotFoundException($"Visit {request.VisitId} was not found.");

        if (visit.Status is VisitStatus.Cancelled or VisitStatus.Completed)
        {
            throw new BusinessRuleViolationException($"A visit with status {visit.Status} cannot be cancelled.");
        }

        visit.Status = VisitStatus.Cancelled;
        visit.CancellationReason = request.Cancellation.CancellationReason;
        visit.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
