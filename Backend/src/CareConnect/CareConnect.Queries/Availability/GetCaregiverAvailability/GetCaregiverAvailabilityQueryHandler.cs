using CareConnect.DTOs.Caregivers;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Common;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Availability.GetCaregiverAvailability;

public sealed class GetCaregiverAvailabilityQueryHandler
    : IRequestHandler<GetCaregiverAvailabilityQuery, IReadOnlyList<CaregiverAvailabilityDto>>
{
    private static readonly string Sql =
        SqlResourceLoader.Load(typeof(GetCaregiverAvailabilityQueryHandler), "GetCaregiverAvailabilityQuery.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public GetCaregiverAvailabilityQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<CaregiverAvailabilityDto>> Handle(
        GetCaregiverAvailabilityQuery request,
        CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var results = await connection.QueryAsync<CaregiverAvailabilityDto>(new CommandDefinition(
            Sql,
            new { request.CaregiverId },
            cancellationToken: cancellationToken));

        return results.ToList();
    }
}
