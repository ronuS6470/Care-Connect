using CareConnect.DTOs.CareTasks;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Common;
using Dapper;
using MediatR;

namespace CareConnect.Queries.CareTasks.GetCareTaskById;

public sealed class GetCareTaskByIdQueryHandler : IRequestHandler<GetCareTaskByIdQuery, CareTaskDto?>
{
    private static readonly string Sql =
        SqlResourceLoader.Load(typeof(GetCareTaskByIdQueryHandler), "GetCareTaskByIdQuery.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public GetCareTaskByIdQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<CareTaskDto?> Handle(GetCareTaskByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<CareTaskDto>(new CommandDefinition(
            Sql,
            new { request.CareTaskId },
            cancellationToken: cancellationToken));
    }
}
