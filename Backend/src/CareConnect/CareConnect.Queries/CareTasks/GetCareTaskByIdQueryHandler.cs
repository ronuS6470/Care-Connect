using CareConnect.DTOs.CareTasks;
using CareConnect.Infrastructure.Data;
using Dapper;
using MediatR;

namespace CareConnect.Queries.CareTasks;

public sealed class GetCareTaskByIdQueryHandler : IRequestHandler<GetCareTaskByIdQuery, CareTaskDto?>
{
    private const string Sql = "SELECT Id, Name, Description, IsActive FROM CareTasks WHERE Id = @CareTaskId;";

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
