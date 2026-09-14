using System.Data;
using Microsoft.Data.SqlClient;

namespace CareConnect.Infrastructure.Data;

/// <summary>
/// Opens plain <see cref="SqlConnection"/>s against the same connection string EF Core is configured
/// with, so command-side (EF Core) and query-side (Dapper) reads/writes always target the same database.
/// </summary>
public sealed class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}
