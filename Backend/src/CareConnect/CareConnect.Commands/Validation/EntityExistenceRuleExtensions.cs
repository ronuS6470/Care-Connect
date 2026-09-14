using CareConnect.Infrastructure.Persistence;
using FluentValidation;

namespace CareConnect.Commands.Validation;

/// <summary>
/// Foreign-key existence checks ("does this Id refer to a real row?") shared by every validator
/// that accepts a related entity's Id. This is still input validation, not a business rule — it's
/// checking that the request references something real, not evaluating a domain policy like
/// availability or scheduling conflicts.
/// </summary>
public static class EntityExistenceRuleExtensions
{
    public static IRuleBuilderOptions<T, int> MustReferenceExisting<T, TEntity>(
        this IRuleBuilder<T, int> ruleBuilder,
        CareConnectDbContext dbContext,
        string entityDisplayName)
        where TEntity : class
    {
        return ruleBuilder
            .MustAsync(async (id, cancellationToken) =>
                await dbContext.Set<TEntity>().FindAsync([id], cancellationToken) is not null)
            .WithMessage($"{{PropertyName}} does not refer to an existing {entityDisplayName}.");
    }
}
