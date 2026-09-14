using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Errors;
using CareConnect.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Commands.Visits;

public sealed class CheckOutVisitCommandHandler : IRequestHandler<CheckOutVisitCommand>
{
    private readonly CareConnectDbContext _dbContext;

    public CheckOutVisitCommandHandler(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(CheckOutVisitCommand request, CancellationToken cancellationToken)
    {
        var visit = await _dbContext.Visits
            .Include(v => v.CaregiverAssignment)
            .FirstOrDefaultAsync(v => v.Id == request.VisitId, cancellationToken)
            ?? throw new NotFoundException($"Visit {request.VisitId} was not found.");

        await VisitOwnership.EnsureCallerOwnsVisitAsync(_dbContext, visit, request.RequestingAuth0UserId, cancellationToken);

        if (visit.Status != VisitStatus.InProgress)
        {
            throw new BusinessRuleViolationException(
                $"Cannot check out: visit status is {visit.Status}, not InProgress.");
        }

        if (visit.ActualStartUtc is null)
        {
            throw new BusinessRuleViolationException("This visit has no recorded check-in time.");
        }

        if (visit.ActualEndUtc is not null)
        {
            throw new BusinessRuleViolationException("This visit has already been checked out.");
        }

        var now = DateTime.UtcNow;

        if (now <= visit.ActualStartUtc.Value)
        {
            throw new BusinessRuleViolationException("Check-out time must be after check-in time.");
        }

        visit.ActualEndUtc = now;
        visit.UpdatedAtUtc = now;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
