using CareConnect.DTOs.Common;
using CareConnect.Infrastructure.Auth;
using CareConnect.Infrastructure.Data;
using CareConnect.Infrastructure.HealthChecks;
using CareConnect.Infrastructure.Persistence;
using CareConnect.Infrastructure.Repositories.Assignments;
using CareConnect.Infrastructure.Repositories.Availability;
using CareConnect.Infrastructure.Repositories.Caregivers;
using CareConnect.Infrastructure.Repositories.CareTasks;
using CareConnect.Infrastructure.Repositories.Clients;
using CareConnect.Infrastructure.Repositories.Visits;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CareConnect.Infrastructure.DependencyInjection;

/// <summary>Composition boundary for persistence, external connectivity, and authentication infrastructure.</summary>
public static class InfrastructureServiceCollectionExtensions
{
    private const int CommandTimeoutSeconds = 30;
    private const int MaxRetryCount = 5;

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
    {
        // Double-gated: requires BOTH a Development environment AND this explicit opt-in, so a
        // stray config value can never bypass real auth outside local testing.
        var bypassAuthForLocalTesting = isDevelopment && configuration.GetValue<bool>("Auth0:BypassForLocalTesting");

        var authenticationBuilder = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

        if (bypassAuthForLocalTesting)
        {
            authenticationBuilder.AddScheme<AuthenticationSchemeOptions, DevelopmentAuthenticationHandler>(
                JwtBearerDefaults.AuthenticationScheme, _ => { });
        }
        else
        {
            var auth0Options = configuration.GetSection(Auth0Options.SectionName).Get<Auth0Options>()
                ?? throw new InvalidOperationException(
                    $"Missing or invalid '{Auth0Options.SectionName}' configuration section.");

            authenticationBuilder.AddJwtBearer(options =>
            {
                options.Authority = $"https://{auth0Options.Domain}/";
                options.Audience = auth0Options.Audience;

                // Keep claim types exactly as Auth0 issues them (e.g. "sub" stays "sub" instead of
                // being remapped to the long ClaimTypes.NameIdentifier URI) — handlers resolve the
                // caller's local User by reading "sub" directly.
                options.MapInboundClaims = false;

                // Missing/invalid/expired bearer token (401) and a valid token that fails a
                // [Authorize(Roles=...)] check (403) both short-circuit inside authentication/
                // authorization middleware — before any controller or GlobalExceptionHandler runs.
                // Without this, ASP.NET Core's default behavior is an empty response body, breaking
                // the "every error uses the same envelope" contract for exactly those two status
                // codes. HandleResponse() on the challenge suppresses the default WWW-Authenticate
                // response so this handler's body isn't overwritten.
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(
                            ApiResponse<object>.Fail("Authentication is required to access this resource."));
                    },
                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(
                            ApiResponse<object>.Fail("You do not have permission to access this resource."));
                    },
                };
            });
        }

        services.AddAuthorization();

        // Single connection string, shared by both EF Core (writes) and Dapper (reads) so the two
        // never drift onto different databases.
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing 'DefaultConnection' connection string.");

        services.AddDbContext<CareConnectDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlServerOptions =>
            {
                sqlServerOptions.EnableRetryOnFailure(
                    maxRetryCount: MaxRetryCount,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
                sqlServerOptions.CommandTimeout(CommandTimeoutSeconds);
                sqlServerOptions.MigrationsAssembly(typeof(CareConnectDbContext).Assembly.FullName);
            });

            // This context is command/write-side only, so tracking every loaded entity is the
            // correct default: updates need EF to diff tracked state to produce their SQL.
            // Read-side projections belong to Dapper, not this context.
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
        });

        services.AddSingleton<IDbConnectionFactory>(_ => new SqlConnectionFactory(connectionString));

        services.AddScoped<ICareTaskRepository, CareTaskRepository>();
        services.AddScoped<ICaregiverAvailabilityRepository, CaregiverAvailabilityRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<ICaregiverRepository, CaregiverRepository>();
        services.AddScoped<IAssignmentRepository, AssignmentRepository>();
        services.AddScoped<IVisitRepository, VisitRepository>();

        services
            .AddHealthChecks()
            .AddCheck<SqlServerHealthCheck>("sql-server", tags: ["db", "ready"]);

        return services;
    }
}
