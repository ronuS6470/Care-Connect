using CareConnect.Infrastructure.Errors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CareConnect.Infrastructure.Persistence;

/// <summary>
/// Serializes "check for a scheduling conflict, then write" for a single caregiver across
/// concurrent requests and across app server instances.
///
/// Why this exists: the double-booking and availability checks are read-then-write. Two requests
/// racing to schedule overlapping visits for the same caregiver can both pass the overlap check
/// (each reads before either writes) and then both insert — a classic TOCTOU race. SQL Server has
/// no native "no overlapping ranges" constraint, and even SERIALIZABLE isolation doesn't reliably
/// range-lock an arbitrary overlap predicate (there's no single index that captures "overlap"),
/// so it can't be trusted alone to close this window.
///
/// The fix: sp_getapplock, SQL Server's cooperative application lock. Call this once, inside the
/// same transaction as the check-then-write, using a resource key scoped to the caregiver
/// ("caregiver-schedule-{id}"). A second concurrent request for the *same* caregiver blocks here
/// until the first transaction commits or rolls back (@LockOwner = 'Transaction' releases it
/// automatically either way), so it always re-reads a state that already includes the first
/// request's write. Requests for *different* caregivers use different resource keys and never
/// contend, so this doesn't serialize scheduling across the whole system — only per caregiver,
/// which is exactly the granularity the business rule needs.
/// </summary>
public static class SchedulingLock
{
    private const int LockTimeoutMilliseconds = 10_000;

    public static async Task AcquireCaregiverScheduleLockAsync(
        DbContext dbContext,
        int caregiverId,
        CancellationToken cancellationToken)
    {
        var transaction = dbContext.Database.CurrentTransaction
            ?? throw new InvalidOperationException(
                "Acquiring the scheduling lock requires an active transaction (call from inside ExecuteInTransactionAsync).");

        var dbTransaction = transaction.GetDbTransaction();
        var connection = dbTransaction.Connection
            ?? throw new InvalidOperationException("The transaction has no associated connection.");

        await using var command = connection.CreateCommand();
        command.Transaction = dbTransaction;
        command.CommandText = """
            DECLARE @lockResult INT;
            EXEC @lockResult = sp_getapplock
                @Resource = @LockResource,
                @LockMode = 'Exclusive',
                @LockOwner = 'Transaction',
                @LockTimeout = @LockTimeoutMilliseconds;
            SELECT @lockResult;
            """;

        var resourceParameter = command.CreateParameter();
        resourceParameter.ParameterName = "@LockResource";
        resourceParameter.Value = $"caregiver-schedule-{caregiverId}";
        command.Parameters.Add(resourceParameter);

        var timeoutParameter = command.CreateParameter();
        timeoutParameter.ParameterName = "@LockTimeoutMilliseconds";
        timeoutParameter.Value = LockTimeoutMilliseconds;
        command.Parameters.Add(timeoutParameter);

        var result = (int)(await command.ExecuteScalarAsync(cancellationToken))!;

        // sp_getapplock return codes: 0/1 = acquired (immediately / after waiting); negative = timeout,
        // cancellation, or deadlock victim.
        if (result < 0)
        {
            throw new BusinessRuleException(
                "Could not acquire the scheduling lock for this caregiver in time; please retry.");
        }
    }
}
