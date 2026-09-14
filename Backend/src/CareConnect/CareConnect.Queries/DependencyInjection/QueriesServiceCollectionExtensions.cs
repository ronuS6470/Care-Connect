using Microsoft.Extensions.DependencyInjection;

namespace CareConnect.Queries.DependencyInjection;

/// <summary>Composition boundary for everything the Queries (read-side) layer owns.</summary>
public static class QueriesServiceCollectionExtensions
{
    public static IServiceCollection AddQueries(this IServiceCollection services)
    {
        // Query handlers are discovered by MediatR's assembly scan (see Program.cs).
        // Read-side-specific registrations (e.g. Dapper type handlers) land here as they're introduced.
        return services;
    }
}
