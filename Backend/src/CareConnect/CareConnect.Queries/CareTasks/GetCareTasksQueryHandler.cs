using CareConnect.DTOs.CareTasks;
using CareConnect.DTOs.Common;
using CareConnect.Infrastructure.Data;
using Dapper;
using MediatR;

namespace CareConnect.Queries.CareTasks;

public sealed class GetCareTasksQueryHandler : IRequestHandler<GetCareTasksQuery, PagedResponseDto<CareTaskDto>>
{
    private const string WhereSql = "WHERE (@IsActive IS NULL OR IsActive = @IsActive)";

    private const string CountSql = $"SELECT COUNT(*) FROM CareTasks {WhereSql};";

    private const string DataSql = $"""
        SELECT Id, Name, Description, IsActive
        FROM CareTasks
        {WhereSql}
        ORDER BY Name
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        """;

    private readonly IDbConnectionFactory _connectionFactory;

    public GetCareTasksQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponseDto<CareTaskDto>> Handle(GetCareTasksQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 200);

        using var connection = _connectionFactory.CreateConnection();

        var parameters = new
        {
            request.IsActive,
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
}
