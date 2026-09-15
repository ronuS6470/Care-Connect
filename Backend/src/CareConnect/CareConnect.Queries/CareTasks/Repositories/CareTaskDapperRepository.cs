using CareConnect.DTOs.CareTasks;
using CareConnect.DTOs.Common;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Common;
using Dapper;

namespace CareConnect.Queries.CareTasks.Repositories;

public sealed class CareTaskDapperRepository : ICareTaskReadRepository
{
    private static readonly string CountSql =
        SqlResourceLoader.Load(typeof(CareTaskDapperRepository).Assembly, "CareTasks.Repositories.Sql.GetCareTasksCount.sql");

    private static readonly string DataSql =
        SqlResourceLoader.Load(typeof(CareTaskDapperRepository).Assembly, "CareTasks.Repositories.Sql.GetCareTasks.sql");

    private static readonly string ByIdSql =
        SqlResourceLoader.Load(typeof(CareTaskDapperRepository).Assembly, "CareTasks.Repositories.Sql.GetCareTaskById.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public CareTaskDapperRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponseDto<CareTaskDto>> GetPagedAsync(int page, int pageSize, bool? isActive, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var parameters = new
        {
            IsActive = isActive,
            Offset = (page - 1) * pageSize,
            PageSize = pageSize,
        };

        var totalRecords = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            CountSql, parameters, cancellationToken: cancellationToken));

        var data = await connection.QueryAsync<CareTaskDto>(new CommandDefinition(
            DataSql, parameters, cancellationToken: cancellationToken));

        return new PagedResponseDto<CareTaskDto>
        {
            Data = data.ToList(),
            CurrentPage = page,
            PageSize = pageSize,
            TotalRecords = totalRecords,
        };
    }

    public async Task<CareTaskDto?> GetByIdAsync(int careTaskId, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<CareTaskDto>(new CommandDefinition(
            ByIdSql, new { CareTaskId = careTaskId }, cancellationToken: cancellationToken));
    }
}
