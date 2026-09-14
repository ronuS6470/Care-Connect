using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Errors;
using CareConnect.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Commands.Visits;

public sealed class CheckInVisitCommandHandler : IRequestHandler<CheckInVisitCommand>
{
    private readonly CareConnectDbContext _dbContext;

    public CheckInVisitCommandHandler(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(CheckInVisitCommand request, CancellationToken cancellationToken)
    {
        var visit = await _dbContext.Visits
            .Include(v => v.CaregiverAssignment)
            .FirstOrDefaultAsync(v => v.Id == request.VisitId, cancellationToken)
            ?? throw new NotFoundException($"Visit {request.VisitId} was not found.");

        await VisitOwnership.EnsureCallerOwnsVisitAsync(_dbContext, visit, request.RequestingAuth0UserId, cancellationToken);

        if (visit.Status != VisitStatus.Scheduled)
        {
            throw new BusinessRuleViolationException(
                $"Cannot check in: visit status is {visit.Status}, not Scheduled.");
        }

        var now = DateTime.UtcNow;
        var earliestAllowed = visit.ScheduledStartUtc.AddMinutes(-VisitExecutionPolicy.CheckInEarlyGraceMinutes);

        if (now < earliestAllowed)
        {
            throw new BusinessRuleViolationException(
                $"Check-in is only allowed within {VisitExecutionPolicy.CheckInEarlyGraceMinutes} minutes of the scheduled start time.");
        }

        visit.ActualStartUtc = now;
        visit.Status = VisitStatus.InProgress;
        visit.UpdatedAtUtc = now;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
