using CareConnect.DTOs.Caregivers;
using CareConnect.Infrastructure.Data;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Availability;

public sealed class GetCaregiverAvailabilityQueryHandler
    : IRequestHandler<GetCaregiverAvailabilityQuery, IReadOnlyList<CaregiverAvailabilityDto>>
{
    private const string Sql = """
        SELECT Id, CaregiverId, DayOfWeek, StartTime, EndTime, IsActive
        FROM CaregiverAvailabilities
        WHERE CaregiverId = @CaregiverId
        ORDER BY DayOfWeek, StartTime;
        """;

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
