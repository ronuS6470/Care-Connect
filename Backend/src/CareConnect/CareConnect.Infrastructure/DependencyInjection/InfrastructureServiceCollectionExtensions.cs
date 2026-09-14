using CareConnect.Infrastructure.Auth;
using CareConnect.Infrastructure.Data;
using CareConnect.Infrastructure.HealthChecks;
using CareConnect.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CareConnect.Infrastructure.DependencyInjection;

/// <summary>Composition boundary for persistence, external connectivity, and authentication infrastructure.</summary>
public static class InfrastructureServiceCollectionExtensions
{
    private const int CommandTimeoutSeconds = 30;
    private const int MaxRetryCount = 5;

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var auth0Options = configuration.GetSection(Auth0Options.SectionName).Get<Auth0Options>()
            ?? throw new InvalidOperationException(
                $"Missing or invalid '{Auth0Options.SectionName}' configuration section.");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"https://{auth0Options.Domain}/";
                options.Audience = auth0Options.Audience;

                // Keep claim types exactly as Auth0 issues them (e.g. "sub" stays "sub" instead of
                // being remapped to the long ClaimTypes.NameIdentifier URI) — handlers resolve the
                // caller's local User by reading "sub" directly.
                options.MapInboundClaims = false;
            });

        services.AddAuthorization();

        // Single connection string, shared by both EF Core (writes) and Dapper (reads) so the two
        // never drift onto different databases.
        var connectionString = configuration.GetConnectionString("CareConnectDatabase")
            ?? throw new InvalidOperationException("Missing 'CareConnectDatabase' connection string.");

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

        services
            .AddHealthChecks()
            .AddCheck<SqlServerHealthCheck>("sql-server", tags: ["db", "ready"]);

        return services;
    }
}
