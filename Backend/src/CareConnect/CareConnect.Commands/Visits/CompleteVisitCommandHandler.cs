using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Errors;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Commands.Visits;

public sealed class CompleteVisitCommandHandler : IRequestHandler<CompleteVisitCommand>
{
    private readonly CareConnectDbContext _dbContext;

    public CompleteVisitCommandHandler(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(CompleteVisitCommand request, CancellationToken cancellationToken)
    {
        var visit = await _dbContext.Visits
            .Include(v => v.CaregiverAssignment)
            .Include(v => v.VisitTasks)
            .FirstOrDefaultAsync(v => v.Id == request.VisitId, cancellationToken)
            ?? throw new NotFoundException($"Visit {request.VisitId} was not found.");

        var caregiver = await VisitOwnership.EnsureCallerOwnsVisitAsync(
            _dbContext, visit, request.RequestingAuth0UserId, cancellationToken);

        // Also blocks Scheduled -> Completed directly: a visit that was never checked in is still
        // Scheduled, so it fails this check before either of the two below is even reached.
        if (visit.Status != VisitStatus.InProgress)
        {
            throw new BusinessRuleViolationException(
                $"Cannot complete: visit status is {visit.Status}, not InProgress.");
        }

        if (visit.ActualStartUtc is null)
        {
            throw new BusinessRuleViolationException("This visit has no recorded check-in time.");
        }

        if (visit.ActualEndUtc is null)
        {
            throw new BusinessRuleViolationException("The visit must be checked out before it can be completed.");
        }

        var incompleteTasks = visit.VisitTasks.Where(t => !t.IsCompleted).ToList();

        if (incompleteTasks.Count > 0)
        {
            if (string.IsNullOrWhiteSpace(request.Completion.IncompleteTasksReason))
            {
                throw new BusinessRuleViolationException(
                    $"{incompleteTasks.Count} visit task(s) are incomplete. Provide IncompleteTasksReason to complete anyway.");
            }

            _dbContext.VisitNotes.Add(new VisitNote
            {
                VisitId = visit.Id,
                AuthorUserId = caregiver.UserId,
                Content = $"Visit completed with {incompleteTasks.Count} incomplete task(s). Reason: {request.Completion.IncompleteTasksReason}",
            });
        }

        visit.Status = VisitStatus.Completed;
        visit.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
