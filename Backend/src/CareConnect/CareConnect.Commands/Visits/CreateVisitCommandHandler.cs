using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Errors;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Commands.Visits;

public sealed class CreateVisitCommandHandler : IRequestHandler<CreateVisitCommand, int>
{
    private readonly CareConnectDbContext _dbContext;

    public CreateVisitCommandHandler(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> Handle(CreateVisitCommand request, CancellationToken cancellationToken) =>
        _dbContext.ExecuteInTransactionAsync(async ct =>
        {
            var dto = request.Visit;

            var assignment = await _dbContext.CaregiverAssignments
                .FirstOrDefaultAsync(a => a.Id == dto.CaregiverAssignmentId, ct)
                ?? throw new NotFoundException($"Caregiver assignment {dto.CaregiverAssignmentId} was not found.");

            // Serializes the check-then-write below against every other scheduling attempt for
            // this same caregiver. See SchedulingLock for why this is needed.
            await SchedulingLock.AcquireCaregiverScheduleLockAsync(_dbContext, assignment.CaregiverId, ct);

            VisitScheduling.EnsureAssignmentActiveForDate(assignment, DateOnly.FromDateTime(dto.ScheduledStartUtc));

            await VisitScheduling.EnsureWithinAvailabilityAsync(
                _dbContext, assignment.CaregiverId, dto.ScheduledStartUtc, dto.ScheduledEndUtc, ct);

            await VisitScheduling.EnsureNoOverlapAsync(
                _dbContext, assignment.CaregiverId, dto.ScheduledStartUtc, dto.ScheduledEndUtc, excludingVisitId: null, ct);

            var visit = new Visit
            {
                CaregiverAssignmentId = dto.CaregiverAssignmentId,
                ScheduledStartUtc = dto.ScheduledStartUtc,
                ScheduledEndUtc = dto.ScheduledEndUtc,
                Status = VisitStatus.Scheduled,
            };

            foreach (var careTaskId in dto.CareTaskIds.Distinct())
            {
                visit.VisitTasks.Add(new VisitTask { CareTaskId = careTaskId, IsCompleted = false });
            }

            _dbContext.Visits.Add(visit);
            await _dbContext.SaveChangesAsync(ct);

            return visit.Id;
        }, cancellationToken);
}
