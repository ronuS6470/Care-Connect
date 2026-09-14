using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Errors;
using CareConnect.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Commands.Visits;

public sealed class UpdateVisitCommandHandler : IRequestHandler<UpdateVisitCommand>
{
    private readonly CareConnectDbContext _dbContext;

    public UpdateVisitCommandHandler(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task Handle(UpdateVisitCommand request, CancellationToken cancellationToken) =>
        _dbContext.ExecuteInTransactionAsync(async ct =>
        {
            var visit = await _dbContext.Visits
                .Include(v => v.CaregiverAssignment)
                .FirstOrDefaultAsync(v => v.Id == request.VisitId, ct)
                ?? throw new NotFoundException($"Visit {request.VisitId} was not found.");

            if (visit.Status != VisitStatus.Scheduled)
            {
                throw new BusinessRuleViolationException(
                    $"A visit with status {visit.Status} cannot be rescheduled.");
            }

            var assignment = visit.CaregiverAssignment;
            var dto = request.Visit;

            await SchedulingLock.AcquireCaregiverScheduleLockAsync(_dbContext, assignment.CaregiverId, ct);

            VisitScheduling.EnsureAssignmentActiveForDate(assignment, DateOnly.FromDateTime(dto.ScheduledStartUtc));

            await VisitScheduling.EnsureWithinAvailabilityAsync(
                _dbContext, assignment.CaregiverId, dto.ScheduledStartUtc, dto.ScheduledEndUtc, ct);

            await VisitScheduling.EnsureNoOverlapAsync(
                _dbContext, assignment.CaregiverId, dto.ScheduledStartUtc, dto.ScheduledEndUtc, visit.Id, ct);

            visit.ScheduledStartUtc = dto.ScheduledStartUtc;
            visit.ScheduledEndUtc = dto.ScheduledEndUtc;
            visit.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(ct);

            return true;
        }, cancellationToken);
}
