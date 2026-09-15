using CareConnect.DTOs.Reporting;
using CareConnect.DTOs.Reports;

namespace CareConnect.AppServices.Reporting;

public interface IReportingAppService
{
    Task<CaregiverEarningsDto> GetCaregiverEarningsAsync(
        int caregiverId, DateOnly fromDate, DateOnly toDate, bool includeVisitBreakdown, CancellationToken cancellationToken);

    Task<IReadOnlyList<WorkedHoursDto>> GetCaregiverHoursAsync(
        int caregiverId, DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken);

    Task<IReadOnlyList<VisitSummaryDto>> GetTodaysVisitsReportAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<VisitSummaryDto>> GetCompletedVisitsReportAsync(DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken);

    Task<IReadOnlyList<VisitsPerCaregiverReportRowDto>> GetVisitsPerCaregiverReportAsync(
        DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken);

    Task<IReadOnlyList<VisitsPerClientReportRowDto>> GetVisitsPerClientReportAsync(
        DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken);

    Task<IReadOnlyList<CaregiverHoursReportRowDto>> GetCaregiverTotalHoursReportAsync(
        DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken);

    Task<IReadOnlyList<CaregiverEarningsReportRowDto>> GetCaregiverTotalEarningsReportAsync(
        DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClientWithoutActiveCaregiverDto>> GetClientsWithoutActiveCaregiverReportAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<CaregiverWithoutVisitsTodayDto>> GetCaregiversWithoutVisitsTodayReportAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<HighestEarningCaregiverReportRowDto>> GetHighestEarningCaregiversReportAsync(
        DateOnly fromDate, DateOnly toDate, int topN, CancellationToken cancellationToken);

    Task<IReadOnlyList<CareTaskDemandReportRowDto>> GetMostRequestedCareTasksReportAsync(
        DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken);

    Task<IReadOnlyList<AverageVisitDurationReportRowDto>> GetAverageVisitDurationReportAsync(
        DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken);

    Task<IReadOnlyList<TopCompletedVisitsCaregiverReportRowDto>> GetTopCaregiversByCompletedVisitsReportAsync(
        DateOnly fromDate, DateOnly toDate, int topN, CancellationToken cancellationToken);

    Task<IReadOnlyList<CaregiverAvailabilityConflictDto>> GetCaregiverAvailabilityConflictsReportAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<TopCaregiverByHoursReportRowDto>> GetTopCaregiversByHoursReportAsync(
        DateOnly fromDate, DateOnly toDate, int topN, CancellationToken cancellationToken);
}
