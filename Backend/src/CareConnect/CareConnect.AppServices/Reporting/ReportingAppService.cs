using CareConnect.AppServices.Security;
using CareConnect.DTOs.Reporting;
using CareConnect.DTOs.Reports;
using CareConnect.Queries.Reporting;
using CareConnect.Queries.Reports;
using MediatR;

namespace CareConnect.AppServices.Reporting;

public sealed class ReportingAppService : IReportingAppService
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public ReportingAppService(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    {
        _mediator = mediator;
        _currentUserAccessor = currentUserAccessor;
    }

    public Task<CaregiverEarningsDto> GetCaregiverEarningsAsync(
        int caregiverId, DateOnly fromDate, DateOnly toDate, bool includeVisitBreakdown, CancellationToken cancellationToken) =>
        _mediator.Send(
            new GetCaregiverEarningsQuery(caregiverId, fromDate, toDate, includeVisitBreakdown, _currentUserAccessor.Auth0UserId),
            cancellationToken);

    public Task<IReadOnlyList<WorkedHoursDto>> GetCaregiverHoursAsync(
        int caregiverId, DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken) =>
        _mediator.Send(new GetCaregiverHoursQuery(caregiverId, fromDate, toDate, _currentUserAccessor.Auth0UserId), cancellationToken);

    public Task<IReadOnlyList<VisitSummaryDto>> GetTodaysVisitsReportAsync(CancellationToken cancellationToken) =>
        _mediator.Send(new GetTodaysVisitsReportQuery(_currentUserAccessor.Auth0UserId), cancellationToken);

    public Task<IReadOnlyList<VisitSummaryDto>> GetCompletedVisitsReportAsync(DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken) =>
        _mediator.Send(new GetCompletedVisitsReportQuery(fromDate, toDate, _currentUserAccessor.Auth0UserId), cancellationToken);

    public Task<IReadOnlyList<VisitsPerCaregiverReportRowDto>> GetVisitsPerCaregiverReportAsync(
        DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken) =>
        _mediator.Send(new GetVisitsPerCaregiverReportQuery(fromDate, toDate, _currentUserAccessor.Auth0UserId), cancellationToken);

    public Task<IReadOnlyList<VisitsPerClientReportRowDto>> GetVisitsPerClientReportAsync(
        DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken) =>
        _mediator.Send(new GetVisitsPerClientReportQuery(fromDate, toDate, _currentUserAccessor.Auth0UserId), cancellationToken);

    public Task<IReadOnlyList<CaregiverHoursReportRowDto>> GetCaregiverTotalHoursReportAsync(
        DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken) =>
        _mediator.Send(new GetCaregiverTotalHoursReportQuery(fromDate, toDate, _currentUserAccessor.Auth0UserId), cancellationToken);

    public Task<IReadOnlyList<CaregiverEarningsReportRowDto>> GetCaregiverTotalEarningsReportAsync(
        DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken) =>
        _mediator.Send(new GetCaregiverTotalEarningsReportQuery(fromDate, toDate, _currentUserAccessor.Auth0UserId), cancellationToken);

    public Task<IReadOnlyList<ClientWithoutActiveCaregiverDto>> GetClientsWithoutActiveCaregiverReportAsync(CancellationToken cancellationToken) =>
        _mediator.Send(new GetClientsWithoutActiveCaregiverReportQuery(_currentUserAccessor.Auth0UserId), cancellationToken);

    public Task<IReadOnlyList<CaregiverWithoutVisitsTodayDto>> GetCaregiversWithoutVisitsTodayReportAsync(CancellationToken cancellationToken) =>
        _mediator.Send(new GetCaregiversWithoutVisitsTodayReportQuery(_currentUserAccessor.Auth0UserId), cancellationToken);

    public Task<IReadOnlyList<HighestEarningCaregiverReportRowDto>> GetHighestEarningCaregiversReportAsync(
        DateOnly fromDate, DateOnly toDate, int topN, CancellationToken cancellationToken) =>
        _mediator.Send(new GetHighestEarningCaregiversReportQuery(fromDate, toDate, topN, _currentUserAccessor.Auth0UserId), cancellationToken);

    public Task<IReadOnlyList<CareTaskDemandReportRowDto>> GetMostRequestedCareTasksReportAsync(
        DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken) =>
        _mediator.Send(new GetMostRequestedCareTasksReportQuery(fromDate, toDate, _currentUserAccessor.Auth0UserId), cancellationToken);

    public Task<IReadOnlyList<AverageVisitDurationReportRowDto>> GetAverageVisitDurationReportAsync(
        DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken) =>
        _mediator.Send(new GetAverageVisitDurationReportQuery(fromDate, toDate, _currentUserAccessor.Auth0UserId), cancellationToken);

    public Task<IReadOnlyList<TopCompletedVisitsCaregiverReportRowDto>> GetTopCaregiversByCompletedVisitsReportAsync(
        DateOnly fromDate, DateOnly toDate, int topN, CancellationToken cancellationToken) =>
        _mediator.Send(new GetTopCaregiversByCompletedVisitsReportQuery(fromDate, toDate, topN, _currentUserAccessor.Auth0UserId), cancellationToken);

    public Task<IReadOnlyList<CaregiverAvailabilityConflictDto>> GetCaregiverAvailabilityConflictsReportAsync(CancellationToken cancellationToken) =>
        _mediator.Send(new GetCaregiverAvailabilityConflictsReportQuery(_currentUserAccessor.Auth0UserId), cancellationToken);

    public Task<IReadOnlyList<TopCaregiverByHoursReportRowDto>> GetTopCaregiversByHoursReportAsync(
        DateOnly fromDate, DateOnly toDate, int topN, CancellationToken cancellationToken) =>
        _mediator.Send(new GetTopCaregiversByHoursReportQuery(fromDate, toDate, topN, _currentUserAccessor.Auth0UserId), cancellationToken);
}
