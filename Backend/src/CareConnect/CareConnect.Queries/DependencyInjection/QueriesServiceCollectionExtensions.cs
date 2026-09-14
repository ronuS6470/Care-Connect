using CareConnect.Queries.Assignments.Repositories;
using CareConnect.Queries.Availability.Repositories;
using CareConnect.Queries.Caregivers.Repositories;
using CareConnect.Queries.CareTasks.Repositories;
using CareConnect.Queries.Clients.Repositories;
using CareConnect.Queries.Visits.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CareConnect.Queries.DependencyInjection;

/// <summary>Composition boundary for everything the Queries (read-side) layer owns.</summary>
public static class QueriesServiceCollectionExtensions
{
    public static IServiceCollection AddQueries(this IServiceCollection services)
    {
        // Query handlers are discovered by MediatR's assembly scan (see Program.cs).
        services.AddScoped<ICareTaskReadRepository, CareTaskDapperRepository>();
        services.AddScoped<ICaregiverAvailabilityReadRepository, CaregiverAvailabilityDapperRepository>();
        services.AddScoped<IClientReadRepository, ClientDapperRepository>();
        services.AddScoped<ICaregiverReadRepository, CaregiverDapperRepository>();
        services.AddScoped<IAssignmentReadRepository, AssignmentDapperRepository>();
        services.AddScoped<IVisitReadRepository, VisitDapperRepository>();

        return services;
    }
}
