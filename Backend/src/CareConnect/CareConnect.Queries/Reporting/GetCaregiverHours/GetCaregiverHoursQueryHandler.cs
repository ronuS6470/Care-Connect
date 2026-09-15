using CareConnect.DTOs.Reporting;
using CareConnect.Infrastructure.Data;
using CareConnect.Infrastructure.Errors;
using CareConnect.Queries.Security;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Reporting.GetCaregiverHours;

public sealed class GetCaregiverHoursQueryHandler : IRequestHandler<GetCaregiverHoursQuery, IReadOnlyList<WorkedHoursDto>>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetCaregiverHoursQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<WorkedHoursDto>> Handle(GetCaregiverHoursQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var requester = await RequesterResolver.ResolveAsync(connection, request.RequestingAuth0UserId, cancellationToken);

        if (!CaregiverEarningsAuthorizer.CanView(requester, request.CaregiverId))
        {
            throw new ForbiddenException("You do not have access to this caregiver's hours.");
        }

        var caregiver = await connection.QuerySingleOrDefaultAsync<EarningsCaregiverRow>(new CommandDefinition(
            ReportingSharedSql.Caregiver, new { request.CaregiverId }, cancellationToken: cancellationToken))
            ?? throw new NotFoundException($"Caregiver {request.CaregiverId} was not found.");

        var visits = await connection.QueryAsync<CaregiverVisitRecord>(new CommandDefinition(
            ReportingSharedSql.WorkedVisits,
            new { request.CaregiverId, request.FromDate, request.ToDate },
            cancellationToken: cancellationToken));

        return CaregiverEarningsCalculator.FilterCompleted(visits)
            .GroupBy(x => DateOnly.FromDateTime(x.CheckInUtc))
            .OrderBy(g => g.Key)
            .Select(g => new WorkedHoursDto
            {
                CaregiverId = request.CaregiverId,
                CaregiverFullName = caregiver.FullName,
                Date = g.Key,
                HoursWorked = CaregiverEarningsCalculator.RoundHours(
                    g.Sum(x => CaregiverEarningsCalculator.CalculateWorkedHours(x.CheckInUtc, x.CheckOutUtc))),
            })
            .ToList();
    }
}
