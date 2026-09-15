using CareConnect.DTOs.Caregivers;
using CareConnect.DTOs.Common;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Common;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Caregivers.GetCaregivers;

public sealed class GetCaregiversQueryHandler : IRequestHandler<GetCaregiversQuery, PagedResponseDto<CaregiverDto>>
{
    private static readonly string CountSql =
        SqlResourceLoader.Load(typeof(GetCaregiversQueryHandler), "GetCaregiversCountQuery.sql");

    private static readonly string DataSql =
        SqlResourceLoader.Load(typeof(GetCaregiversQueryHandler), "GetCaregiversQuery.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public GetCaregiversQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponseDto<CaregiverDto>> Handle(GetCaregiversQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

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
}
