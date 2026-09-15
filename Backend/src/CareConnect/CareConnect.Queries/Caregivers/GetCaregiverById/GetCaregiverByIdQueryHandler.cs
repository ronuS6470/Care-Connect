using CareConnect.DTOs.Caregivers;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Common;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Caregivers.GetCaregiverById;

public sealed class GetCaregiverByIdQueryHandler : IRequestHandler<GetCaregiverByIdQuery, CaregiverDto?>
{
    private static readonly string Sql =
        SqlResourceLoader.Load(typeof(GetCaregiverByIdQueryHandler), "GetCaregiverByIdQuery.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public GetCaregiverByIdQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<CaregiverDto?> Handle(GetCaregiverByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<CaregiverDto>(new CommandDefinition(
            Sql,
            new { request.CaregiverId },
            cancellationToken: cancellationToken));
    }
}
