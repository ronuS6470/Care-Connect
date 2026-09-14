using CareConnect.DTOs.Caregivers;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Common;
using Dapper;

namespace CareConnect.Queries.Availability.Repositories;

public sealed class CaregiverAvailabilityDapperRepository : ICaregiverAvailabilityReadRepository
{
    private static readonly string GetByCaregiverIdSql =
        SqlResourceLoader.Load(typeof(CaregiverAvailabilityDapperRepository).Assembly, "Availability.Repositories.Sql.GetByCaregiverId.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public CaregiverAvailabilityDapperRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<CaregiverAvailabilityDto>> GetByCaregiverIdAsync(int caregiverId, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var results = await connection.QueryAsync<CaregiverAvailabilityDto>(new CommandDefinition(
            GetByCaregiverIdSql,
            new { CaregiverId = caregiverId },
            cancellationToken: cancellationToken));

        return results.ToList();
    }
}
