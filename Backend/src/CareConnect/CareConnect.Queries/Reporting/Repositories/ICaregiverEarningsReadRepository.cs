using CareConnect.DTOs.Reporting;

namespace CareConnect.Queries.Reporting.Repositories;

public interface ICaregiverEarningsReadRepository
{
    Task<CaregiverEarningsDto> GetEarningsAsync(
        int caregiverId,
        DateOnly fromDate,
        DateOnly toDate,
        bool includeVisitBreakdown,
        string requestingAuth0UserId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<WorkedHoursDto>> GetHoursByDateAsync(
        int caregiverId,
        DateOnly fromDate,
        DateOnly toDate,
        string requestingAuth0UserId,
        CancellationToken cancellationToken);
}
