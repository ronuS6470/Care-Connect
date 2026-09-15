using CareConnect.DTOs.Reporting;
using CareConnect.DTOs.Reports;

namespace CareConnect.Queries.Reports.Repositories;

/// <summary>Admin-only operational reports. Every method is Admin-checked server-side (defense in depth beyond [Authorize(Roles=Admin)] on the controller).</summary>
public interface IReportsReadRepository
{
    Task<IReadOnlyList<VisitSummaryDto>> GetTodaysVisitsAsync(string requestingAuth0UserId, CancellationToken cancellationToken);

    Task<IReadOnlyList<VisitSummaryDto>> GetCompletedVisitsAsync(
        DateOnly fromDate, DateOnly toDate, string requestingAuth0UserId, CancellationToken cancellationToken);

    Task<IReadOnlyList<VisitsPerCaregiverReportRowDto>> GetVisitsPerCaregiverAsync(
        DateOnly fromDate, DateOnly toDate, string requestingAuth0UserId, CancellationToken cancellationToken);

    Task<IReadOnlyList<VisitsPerClientReportRowDto>> GetVisitsPerClientAsync(
        DateOnly fromDate, DateOnly toDate, string requestingAuth0UserId, CancellationToken cancellationToken);

    Task<IReadOnlyList<CaregiverHoursReportRowDto>> GetCaregiverTotalHoursAsync(
        DateOnly fromDate, DateOnly toDate, string requestingAuth0UserId, CancellationToken cancellationToken);

    Task<IReadOnlyList<CaregiverEarningsReportRowDto>> GetCaregiverTotalEarningsAsync(
        DateOnly fromDate, DateOnly toDate, string requestingAuth0UserId, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClientWithoutActiveCaregiverDto>> GetClientsWithoutActiveCaregiverAsync(
        string requestingAuth0UserId, CancellationToken cancellationToken);

    Task<IReadOnlyList<CaregiverWithoutVisitsTodayDto>> GetCaregiversWithoutVisitsTodayAsync(
        string requestingAuth0UserId, CancellationToken cancellationToken);

    Task<IReadOnlyList<HighestEarningCaregiverReportRowDto>> GetHighestEarningCaregiversAsync(
        DateOnly fromDate, DateOnly toDate, int topN, string requestingAuth0UserId, CancellationToken cancellationToken);

    Task<IReadOnlyList<CareTaskDemandReportRowDto>> GetMostRequestedCareTasksAsync(
        DateOnly fromDate, DateOnly toDate, string requestingAuth0UserId, CancellationToken cancellationToken);

    Task<IReadOnlyList<AverageVisitDurationReportRowDto>> GetAverageVisitDurationAsync(
        DateOnly fromDate, DateOnly toDate, string requestingAuth0UserId, CancellationToken cancellationToken);

    Task<IReadOnlyList<TopCompletedVisitsCaregiverReportRowDto>> GetTopCaregiversByCompletedVisitsAsync(
        DateOnly fromDate, DateOnly toDate, int topN, string requestingAuth0UserId, CancellationToken cancellationToken);

    Task<IReadOnlyList<CaregiverAvailabilityConflictDto>> GetCaregiverAvailabilityConflictsAsync(
        string requestingAuth0UserId, CancellationToken cancellationToken);

    Task<IReadOnlyList<TopCaregiverByHoursReportRowDto>> GetTopCaregiversByHoursAsync(
        DateOnly fromDate, DateOnly toDate, int topN, string requestingAuth0UserId, CancellationToken cancellationToken);
}
