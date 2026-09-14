using Microsoft.EntityFrameworkCore;

namespace CareConnect.Infrastructure.Persistence;

/// <summary>
/// Lets command handlers wrap a multi-write business operation in a single local transaction.
/// Single-write handlers don't need this — one <c>SaveChangesAsync</c> call is already atomic.
/// </summary>
public static class DbContextTransactionExtensions
{
    public static async Task<TResult> ExecuteInTransactionAsync<TResult>(
        this CareConnectDbContext dbContext,
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken = default)
    {
        // EnableRetryOnFailure requires retries to replay the whole transaction, not just
        // individual SaveChanges calls, hence going through the execution strategy here.
        var strategy = dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            var result = await operation(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return result;
        });
    }
}
