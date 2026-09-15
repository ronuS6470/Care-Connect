using CareConnect.DTOs.Caregivers;
using CareConnect.DTOs.Common;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Common;
using Dapper;

namespace CareConnect.Queries.Caregivers.Repositories;

public sealed class CaregiverDapperRepository : ICaregiverReadRepository
{
    private static readonly string CountSql =
        SqlResourceLoader.Load(typeof(CaregiverDapperRepository).Assembly, "Caregivers.Repositories.Sql.GetCaregiversCount.sql");

    private static readonly string DataSql =
        SqlResourceLoader.Load(typeof(CaregiverDapperRepository).Assembly, "Caregivers.Repositories.Sql.GetCaregivers.sql");

    private static readonly string ByIdSql =
        SqlResourceLoader.Load(typeof(CaregiverDapperRepository).Assembly, "Caregivers.Repositories.Sql.GetCaregiverById.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public CaregiverDapperRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponseDto<CaregiverDto>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var totalRecords = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(CountSql, cancellationToken: cancellationToken));

        var data = await connection.QueryAsync<CaregiverDto>(new CommandDefinition(
            DataSql,
            new { Offset = (page - 1) * pageSize, PageSize = pageSize },
            cancellationToken: cancellationToken));

        return new PagedResponseDto<CaregiverDto>
        {
            Data = data.ToList(),
            CurrentPage = page,
            PageSize = pageSize,
            TotalRecords = totalRecords,
        };
    }

    public async Task<CaregiverDto?> GetByIdAsync(int caregiverId, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<CaregiverDto>(new CommandDefinition(
            ByIdSql,
            new { CaregiverId = caregiverId },
            cancellationToken: cancellationToken));
    }
}
