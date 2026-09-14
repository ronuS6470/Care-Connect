using System.Data;

namespace CareConnect.Infrastructure.Data;

/// <summary>Creates ADO.NET connections for Dapper query handlers against the CareConnect database.</summary>
public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
