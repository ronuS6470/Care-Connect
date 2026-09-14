using CareConnect.DTOs.Caregivers;
using CareConnect.Infrastructure.Data;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Caregivers;

public sealed class GetCaregiverByIdQueryHandler : IRequestHandler<GetCaregiverByIdQuery, CaregiverDto?>
{
    private const string Sql = """
        SELECT c.Id, c.UserId, u.FirstName + ' ' + u.LastName AS FullName, u.Email, u.PhoneNumber,
               c.LicenseNumber, c.HourlyRate, c.HireDate, c.DateOfBirth, c.YearsOfExperience, c.IsActive
        FROM Caregivers c
        INNER JOIN Users u ON u.Id = c.UserId
        WHERE c.Id = @CaregiverId;
        """;

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
