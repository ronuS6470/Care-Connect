using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CareConnect.Infrastructure.Persistence;

/// <summary>
/// Lets `dotnet ef` generate migrations for this class library without needing the ASP.NET Core
/// host's DI container. Only used at design time, never at application runtime.
/// </summary>
public class CareConnectDbContextFactory : IDesignTimeDbContextFactory<CareConnectDbContext>
{
    public CareConnectDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("CARECONNECT_CONNECTION_STRING")
            ?? "Server=(localdb)\\mssqllocaldb;Database=CareConnectDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

        var optionsBuilder = new DbContextOptionsBuilder<CareConnectDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new CareConnectDbContext(optionsBuilder.Options);
    }
}
