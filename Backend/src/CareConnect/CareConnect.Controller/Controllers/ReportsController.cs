using CareConnect.AppServices.Reporting;
using CareConnect.DTOs.Reporting;
using CareConnect.DTOs.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareConnect.Controller.Controllers;

/// <summary>Admin-only operational reports — distinct from ReportingController's self-service caregiver earnings/hours endpoints.</summary>
[ApiController]
[Route("api/reports")]
[Authorize(Roles = "Admin")]
public sealed class ReportsController : ControllerBase
{
    private const int DefaultTopN = 10;

    private readonly IReportingAppService _reportingAppService;

    public ReportsController(IReportingAppService reportingAppService)
    {
        _reportingAppService = reportingAppService;
    }

    [HttpGet("todays-visits")]
    public async Task<ActionResult<IReadOnlyList<VisitSummaryDto>>> GetTodaysVisits(CancellationToken cancellationToken)
    {
        var result = await _reportingAppService.GetTodaysVisitsReportAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("completed-visits")]
    public async Task<ActionResult<IReadOnlyList<VisitSummaryDto>>> GetCompletedVisits(
        [FromQuery] DateOnly fromDate, [FromQuery] DateOnly toDate, CancellationToken cancellationToken)
    {
        var result = await _reportingAppService.GetCompletedVisitsReportAsync(fromDate, toDate, cancellationToken);
        return Ok(result);
    }

    [HttpGet("visits-per-caregiver")]
    public async Task<ActionResult<IReadOnlyList<VisitsPerCaregiverReportRowDto>>> GetVisitsPerCaregiver(
        [FromQuery] DateOnly fromDate, [FromQuery] DateOnly toDate, CancellationToken cancellationToken)
    {
        var result = await _reportingAppService.GetVisitsPerCaregiverReportAsync(fromDate, toDate, cancellationToken);
        return Ok(result);
    }

    [HttpGet("visits-per-client")]
    public async Task<ActionResult<IReadOnlyList<VisitsPerClientReportRowDto>>> GetVisitsPerClient(
        [FromQuery] DateOnly fromDate, [FromQuery] DateOnly toDate, CancellationToken cancellationToken)
    {
        var result = await _reportingAppService.GetVisitsPerClientReportAsync(fromDate, toDate, cancellationToken);
        return Ok(result);
    }

    [HttpGet("caregiver-hours")]
    public async Task<ActionResult<IReadOnlyList<CaregiverHoursReportRowDto>>> GetCaregiverTotalHours(
        [FromQuery] DateOnly fromDate, [FromQuery] DateOnly toDate, CancellationToken cancellationToken)
    {
        var result = await _reportingAppService.GetCaregiverTotalHoursReportAsync(fromDate, toDate, cancellationToken);
        return Ok(result);
    }

    [HttpGet("caregiver-earnings")]
    public async Task<ActionResult<IReadOnlyList<CaregiverEarningsReportRowDto>>> GetCaregiverTotalEarnings(
        [FromQuery] DateOnly fromDate, [FromQuery] DateOnly toDate, CancellationToken cancellationToken)
    {
        var result = await _reportingAppService.GetCaregiverTotalEarningsReportAsync(fromDate, toDate, cancellationToken);
        return Ok(result);
    }

    [HttpGet("clients-without-active-caregiver")]
    public async Task<ActionResult<IReadOnlyList<ClientWithoutActiveCaregiverDto>>> GetClientsWithoutActiveCaregiver(
        CancellationToken cancellationToken)
    {
        var result = await _reportingAppService.GetClientsWithoutActiveCaregiverReportAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("caregivers-without-visits-today")]
    public async Task<ActionResult<IReadOnlyList<CaregiverWithoutVisitsTodayDto>>> GetCaregiversWithoutVisitsToday(
        CancellationToken cancellationToken)
    {
        var result = await _reportingAppService.GetCaregiversWithoutVisitsTodayReportAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("highest-earning-caregivers")]
    public async Task<ActionResult<IReadOnlyList<HighestEarningCaregiverReportRowDto>>> GetHighestEarningCaregivers(
        [FromQuery] DateOnly fromDate, [FromQuery] DateOnly toDate, [FromQuery] int topN = DefaultTopN,
        CancellationToken cancellationToken = default)
    {
        var result = await _reportingAppService.GetHighestEarningCaregiversReportAsync(fromDate, toDate, topN, cancellationToken);
        return Ok(result);
    }

    [HttpGet("most-requested-care-tasks")]
    public async Task<ActionResult<IReadOnlyList<CareTaskDemandReportRowDto>>> GetMostRequestedCareTasks(
        [FromQuery] DateOnly fromDate, [FromQuery] DateOnly toDate, CancellationToken cancellationToken)
    {
        var result = await _reportingAppService.GetMostRequestedCareTasksReportAsync(fromDate, toDate, cancellationToken);
        return Ok(result);
    }

    [HttpGet("average-visit-duration")]
    public async Task<ActionResult<IReadOnlyList<AverageVisitDurationReportRowDto>>> GetAverageVisitDuration(
        [FromQuery] DateOnly fromDate, [FromQuery] DateOnly toDate, CancellationToken cancellationToken)
    {
        var result = await _reportingAppService.GetAverageVisitDurationReportAsync(fromDate, toDate, cancellationToken);
        return Ok(result);
    }

    [HttpGet("top-caregivers-by-completed-visits")]
    public async Task<ActionResult<IReadOnlyList<TopCompletedVisitsCaregiverReportRowDto>>> GetTopCaregiversByCompletedVisits(
        [FromQuery] DateOnly fromDate, [FromQuery] DateOnly toDate, [FromQuery] int topN = DefaultTopN,
        CancellationToken cancellationToken = default)
    {
        var result = await _reportingAppService.GetTopCaregiversByCompletedVisitsReportAsync(fromDate, toDate, topN, cancellationToken);
        return Ok(result);
    }

    [HttpGet("caregiver-availability-conflicts")]
    public async Task<ActionResult<IReadOnlyList<CaregiverAvailabilityConflictDto>>> GetCaregiverAvailabilityConflicts(
        CancellationToken cancellationToken)
    {
        var result = await _reportingAppService.GetCaregiverAvailabilityConflictsReportAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("top-caregivers-by-hours")]
    public async Task<ActionResult<IReadOnlyList<TopCaregiverByHoursReportRowDto>>> GetTopCaregiversByHours(
        [FromQuery] DateOnly fromDate, [FromQuery] DateOnly toDate, [FromQuery] int topN = DefaultTopN,
        CancellationToken cancellationToken = default)
    {
        var result = await _reportingAppService.GetTopCaregiversByHoursReportAsync(fromDate, toDate, topN, cancellationToken);
        return Ok(result);
    }
}
