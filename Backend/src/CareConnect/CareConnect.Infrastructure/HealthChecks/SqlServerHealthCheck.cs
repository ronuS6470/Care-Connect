using System.Data.Common;
using CareConnect.Infrastructure.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CareConnect.Infrastructure.HealthChecks;

/// <summary>Reports whether the configured SQL Server database is reachable.</summary>
public sealed class SqlServerHealthCheck : IHealthCheck
{
    private readonly IDbConnectionFactory _connectionFactory;

    public SqlServerHealthCheck(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = (DbConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync(cancellationToken);

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            await command.ExecuteScalarAsync(cancellationToken);

            return HealthCheckResult.Healthy("SQL Server connection succeeded.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("SQL Server connection failed.", ex);
        }
    }
}
