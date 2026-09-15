using CareConnect.DTOs.Enums;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Errors;
using CareConnect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Infrastructure.Repositories.Visits;

public sealed class VisitRepository : IVisitRepository
{
    private readonly CareConnectDbContext _dbContext;

    public VisitRepository(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> CreateScheduledVisitAsync(
        int caregiverAssignmentId,
        DateTime scheduledStartUtc,
        DateTime scheduledEndUtc,
        IReadOnlyCollection<int> careTaskIds,
        CancellationToken cancellationToken) =>
        _dbContext.ExecuteInTransactionAsync(async ct =>
        {
            var assignment = await _dbContext.CaregiverAssignments
                .FirstOrDefaultAsync(a => a.Id == caregiverAssignmentId, ct)
                ?? throw new NotFoundException($"Caregiver assignment {caregiverAssignmentId} was not found.");

            // Serializes the check-then-write below against every other scheduling attempt for
            // this same caregiver. See SchedulingLock for why this is needed.
            await SchedulingLock.AcquireCaregiverScheduleLockAsync(_dbContext, assignment.CaregiverId, ct);

            EnsureAssignmentActiveForDate(assignment, DateOnly.FromDateTime(scheduledStartUtc));

            await EnsureWithinAvailabilityAsync(assignment.CaregiverId, scheduledStartUtc, scheduledEndUtc, ct);

            await EnsureNoOverlapAsync(assignment.CaregiverId, scheduledStartUtc, scheduledEndUtc, excludingVisitId: null, ct);

            var visit = new Visit
            {
                CaregiverAssignmentId = caregiverAssignmentId,
                ScheduledStartUtc = scheduledStartUtc,
                ScheduledEndUtc = scheduledEndUtc,
                Status = VisitStatus.Scheduled,
            };

            foreach (var careTaskId in careTaskIds.Distinct())
            {
                visit.VisitTasks.Add(new VisitTask { CareTaskId = careTaskId, IsCompleted = false });
            }

            _dbContext.Visits.Add(visit);
            await _dbContext.SaveChangesAsync(ct);

            return visit.Id;
        }, cancellationToken);

    public Task RescheduleVisitAsync(
        int visitId,
        DateTime scheduledStartUtc,
        DateTime scheduledEndUtc,
        CancellationToken cancellationToken) =>
        _dbContext.ExecuteInTransactionAsync(async ct =>
        {
            var visit = await _dbContext.Visits
                .Include(v => v.CaregiverAssignment)
                .FirstOrDefaultAsync(v => v.Id == visitId, ct)
                ?? throw new NotFoundException($"Visit {visitId} was not found.");

            if (visit.Status != VisitStatus.Scheduled)
            {
                throw new BusinessRuleException(
                    $"A visit with status {visit.Status} cannot be rescheduled.");
            }

            var assignment = visit.CaregiverAssignment;

            await SchedulingLock.AcquireCaregiverScheduleLockAsync(_dbContext, assignment.CaregiverId, ct);

            EnsureAssignmentActiveForDate(assignment, DateOnly.FromDateTime(scheduledStartUtc));

            await EnsureWithinAvailabilityAsync(assignment.CaregiverId, scheduledStartUtc, scheduledEndUtc, ct);

            await EnsureNoOverlapAsync(assignment.CaregiverId, scheduledStartUtc, scheduledEndUtc, visit.Id, ct);

            visit.ScheduledStartUtc = scheduledStartUtc;
            visit.ScheduledEndUtc = scheduledEndUtc;
            visit.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(ct);

            return true;
        }, cancellationToken);

    public Task<Visit?> GetByIdAsync(int visitId, CancellationToken cancellationToken) =>
        _dbContext.Visits
            .Include(v => v.CaregiverAssignment)
            .FirstOrDefaultAsync(v => v.Id == visitId, cancellationToken);

    public Task<Visit?> GetByIdWithTasksAsync(int visitId, CancellationToken cancellationToken) =>
        _dbContext.Visits
            .Include(v => v.CaregiverAssignment)
            .Include(v => v.VisitTasks)
            .FirstOrDefaultAsync(v => v.Id == visitId, cancellationToken);

    public async Task<(Visit Visit, VisitTask Task)> GetTaskAsync(int visitId, int visitTaskId, CancellationToken cancellationToken)
    {
        var visit = await _dbContext.Visits
            .Include(v => v.CaregiverAssignment)
            .FirstOrDefaultAsync(v => v.Id == visitId, cancellationToken)
            ?? throw new NotFoundException($"Visit {visitId} was not found.");

        var task = await _dbContext.VisitTasks
            .FirstOrDefaultAsync(t => t.Id == visitTaskId, cancellationToken)
            ?? throw new NotFoundException($"Visit task {visitTaskId} was not found.");

        if (task.VisitId != visitId)
        {
            throw new NotFoundException($"Visit task {visitTaskId} was not found on visit {visitId}.");
        }

        return (visit, task);
    }

    public Task<Caregiver?> GetCaregiverByAuth0UserIdAsync(string auth0UserId, CancellationToken cancellationToken) =>
        _dbContext.Caregivers.FirstOrDefaultAsync(c => c.User.Auth0UserId == auth0UserId, cancellationToken);

    public Task<User?> GetUserByAuth0UserIdAsync(string auth0UserId, CancellationToken cancellationToken) =>
        _dbContext.Users.FirstOrDefaultAsync(u => u.Auth0UserId == auth0UserId, cancellationToken);

    public void AddVisitNote(VisitNote note) => _dbContext.VisitNotes.Add(note);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);

    /// <summary>Rules 3 &amp; 5: the assignment must be Active and cover the visit's date.</summary>
    private static void EnsureAssignmentActiveForDate(CaregiverAssignment assignment, DateOnly visitDate)
    {
        if (assignment.Status != AssignmentStatus.Active)
        {
            throw new BusinessRuleException("The assignment must be active to schedule a visit against it.");
        }

        if (visitDate < assignment.StartDate || (assignment.EndDate.HasValue && visitDate > assignment.EndDate.Value))
        {
            throw new BusinessRuleException("The assignment is not active for the visit's date.");
        }
    }

    /// <summary>Rule 1: the visit must fall entirely inside one of the caregiver's availability windows.</summary>
    private async Task EnsureWithinAvailabilityAsync(
        int caregiverId,
        DateTime scheduledStartUtc,
        DateTime scheduledEndUtc,
        CancellationToken cancellationToken)
    {
        var dayOfWeek = scheduledStartUtc.DayOfWeek;
        var startTime = TimeOnly.FromDateTime(scheduledStartUtc);
        var endTime = TimeOnly.FromDateTime(scheduledEndUtc);

        var fitsWithinAWindow = await _dbContext.CaregiverAvailabilities.AnyAsync(a =>
            a.CaregiverId == caregiverId &&
            a.DayOfWeek == dayOfWeek &&
            a.IsActive &&
            a.StartTime <= startTime &&
            a.EndTime >= endTime,
            cancellationToken);

        if (!fitsWithinAWindow)
        {
            throw new BusinessRuleException(
                "The visit falls outside the caregiver's configured availability for that day.");
        }
    }

    /// <summary>Rule 2: no overlapping visit for the same caregiver, across any of their assignments.</summary>
    private async Task EnsureNoOverlapAsync(
        int caregiverId,
        DateTime scheduledStartUtc,
        DateTime scheduledEndUtc,
        int? excludingVisitId,
        CancellationToken cancellationToken)
    {
        var hasOverlap = await _dbContext.Visits.AnyAsync(v =>
            v.Id != (excludingVisitId ?? -1) &&
            v.CaregiverAssignment.CaregiverId == caregiverId &&
            v.Status != VisitStatus.Cancelled &&
            v.ScheduledStartUtc < scheduledEndUtc &&
            v.ScheduledEndUtc > scheduledStartUtc,
            cancellationToken);

        if (hasOverlap)
        {
            throw new ConflictException("Caregiver is already booked during this time.");
        }
    }
}
