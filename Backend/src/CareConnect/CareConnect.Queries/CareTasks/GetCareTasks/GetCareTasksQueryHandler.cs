using CareConnect.DTOs.CareTasks;
using CareConnect.DTOs.Common;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Common;
using Dapper;
using MediatR;

namespace CareConnect.Queries.CareTasks.GetCareTasks;

public sealed class GetCareTasksQueryHandler : IRequestHandler<GetCareTasksQuery, PagedResponseDto<CareTaskDto>>
{
    private static readonly string CountSql =
        SqlResourceLoader.Load(typeof(GetCareTasksQueryHandler), "GetCareTasksCountQuery.sql");

    private static readonly string DataSql =
        SqlResourceLoader.Load(typeof(GetCareTasksQueryHandler), "GetCareTasksQuery.sql");

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
