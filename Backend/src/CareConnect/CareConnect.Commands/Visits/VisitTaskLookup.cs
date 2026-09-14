using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Errors;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Commands.Visits;

/// <summary>
/// Loads a visit + one of its tasks and verifies the task actually belongs to that visit — a
/// mismatch (a real task Id nested under the wrong visit Id) is reported as not-found rather than
/// forbidden, so it doesn't confirm the task's existence to a caller who guessed its Id.
/// </summary>
internal static class VisitTaskLookup
{
    public static async Task<(Visit Visit, VisitTask Task)> LoadAsync(
        CareConnectDbContext dbContext,
        int visitId,
        int visitTaskId,
        CancellationToken cancellationToken)
    {
        var visit = await dbContext.Visits
            .Include(v => v.CaregiverAssignment)
            .FirstOrDefaultAsync(v => v.Id == visitId, cancellationToken)
            ?? throw new NotFoundException($"Visit {visitId} was not found.");

        var task = await dbContext.VisitTasks
            .FirstOrDefaultAsync(t => t.Id == visitTaskId, cancellationToken)
            ?? throw new NotFoundException($"Visit task {visitTaskId} was not found.");

        if (task.VisitId != visitId)
        {
            throw new NotFoundException($"Visit task {visitTaskId} was not found on visit {visitId}.");
        }

        return (visit, task);
    }

    public static void EnsureVisitIsEditable(Visit visit)
    {
        if (visit.Status is VisitStatus.Completed or VisitStatus.Cancelled or VisitStatus.NoShow)
        {
            throw new BusinessRuleViolationException($"Cannot modify tasks on a visit with status {visit.Status}.");
        }
    }
}
