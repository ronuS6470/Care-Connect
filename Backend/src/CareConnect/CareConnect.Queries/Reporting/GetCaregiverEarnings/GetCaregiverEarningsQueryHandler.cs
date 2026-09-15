using CareConnect.DTOs.Reporting;
using CareConnect.Infrastructure.Data;
using CareConnect.Infrastructure.Errors;
using CareConnect.Queries.Security;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Reporting.GetCaregiverEarnings;

public sealed class GetCaregiverEarningsQueryHandler : IRequestHandler<GetCaregiverEarningsQuery, CaregiverEarningsDto>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetCaregiverEarningsQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<CaregiverEarningsDto> Handle(GetCaregiverEarningsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var requester = await RequesterResolver.ResolveAsync(connection, request.RequestingAuth0UserId, cancellationToken);

        if (!CaregiverEarningsAuthorizer.CanView(requester, request.CaregiverId))
        {
            throw new ForbiddenException("You do not have access to this caregiver's earnings.");
        }

        var caregiver = await connection.QuerySingleOrDefaultAsync<EarningsCaregiverRow>(new CommandDefinition(
            ReportingSharedSql.Caregiver, new { request.CaregiverId }, cancellationToken: cancellationToken))
            ?? throw new NotFoundException($"Caregiver {request.CaregiverId} was not found.");

        var visits = await connection.QueryAsync<CaregiverVisitRecord>(new CommandDefinition(
            ReportingSharedSql.WorkedVisits,
            new { request.CaregiverId, request.FromDate, request.ToDate },
            cancellationToken: cancellationToken));

        var (totalHours, totalEarnings, completedCount, lines) = CaregiverEarningsCalculator.Calculate(visits, caregiver.HourlyRate);

        return new CaregiverEarningsDto
        {
            CaregiverId = request.CaregiverId,
            CaregiverFullName = caregiver.FullName,
            PeriodStart = request.FromDate,
            PeriodEnd = request.ToDate,
            HourlyRate = caregiver.HourlyRate,
            TotalHoursWorked = totalHours,
            TotalEarnings = totalEarnings,
            CompletedVisitCount = completedCount,
            Visits = request.IncludeVisitBreakdown ? lines : null,
        };
    }
}
