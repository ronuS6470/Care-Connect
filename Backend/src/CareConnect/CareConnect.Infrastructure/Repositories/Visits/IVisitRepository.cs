using CareConnect.Infrastructure.Entities;

namespace CareConnect.Infrastructure.Repositories.Visits;

public interface IVisitRepository
{
    /// <summary>
    /// Schedules a new visit. Owns the transaction, the per-caregiver scheduling lock, and the
    /// scheduling-rule checks internally — callers must NOT wrap this in their own transaction or
    /// call the lock themselves, since the lock only serializes correctly when it, the checks, and
    /// the write all share one connection/transaction. Throws NotFoundException /
    /// BusinessRuleViolationException on the same conditions as before this was a repository method.
    /// </summary>
    Task<int> CreateScheduledVisitAsync(
        int caregiverAssignmentId,
        DateTime scheduledStartUtc,
        DateTime scheduledEndUtc,
        IReadOnlyCollection<int> careTaskIds,
        CancellationToken cancellationToken);

    /// <summary>Reschedules a Scheduled visit. Same atomicity contract as <see cref="CreateScheduledVisitAsync"/>.</summary>
    Task RescheduleVisitAsync(
        int visitId,
        DateTime scheduledStartUtc,
        DateTime scheduledEndUtc,
        CancellationToken cancellationToken);

    /// <summary>Tracked visit including its CaregiverAssignment, for ownership/access checks.</summary>
    Task<Visit?> GetByIdAsync(int visitId, CancellationToken cancellationToken);

    /// <summary>Tracked visit including CaregiverAssignment and VisitTasks.</summary>
    Task<Visit?> GetByIdWithTasksAsync(int visitId, CancellationToken cancellationToken);

    /// <summary>
    /// Loads a visit + one of its tasks, verifying the task belongs to that visit. Throws
    /// NotFoundException (not ForbiddenException) on a visit/task mismatch, so a caller who guessed
    /// a real task Id under the wrong visit Id learns nothing about its existence.
    /// </summary>
    Task<(Visit Visit, VisitTask Task)> GetTaskAsync(int visitId, int visitTaskId, CancellationToken cancellationToken);

    Task<Caregiver?> GetCaregiverByAuth0UserIdAsync(string auth0UserId, CancellationToken cancellationToken);

    Task<User?> GetUserByAuth0UserIdAsync(string auth0UserId, CancellationToken cancellationToken);

    void AddVisitNote(VisitNote note);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
