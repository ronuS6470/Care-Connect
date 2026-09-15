using CareConnect.AppServices.Reporting;
using CareConnect.DTOs.Reporting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareConnect.Controller.Controllers;

/// <summary>
/// Caregiver earnings/hours reporting. [Authorize(Roles=...)] blocks Clients outright; the
/// handler additionally enforces that a Caregiver may only view their own data (Admin sees any
/// caregiver's) — see CaregiverEarningsAuthorizer.
/// </summary>
[ApiController]
[Route("api/reporting")]
[Authorize(Roles = "Admin,Caregiver")]
public sealed class ReportingController : ControllerBase
{
    private readonly IReportingAppService _reportingAppService;

    public ReportingController(IReportingAppService reportingAppService)
    {
        _reportingAppService = reportingAppService;
    }

    [HttpGet("caregivers/{caregiverId:int}/earnings")]
    public async Task<ActionResult<CaregiverEarningsDto>> GetCaregiverEarnings(
        int caregiverId,
        [FromQuery] DateOnly fromDate,
        [FromQuery] DateOnly toDate,
        [FromQuery] bool includeVisitBreakdown = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _reportingAppService.GetCaregiverEarningsAsync(caregiverId, fromDate, toDate, includeVisitBreakdown, cancellationToken);
        return Ok(result);
    }

    [HttpGet("caregivers/{caregiverId:int}/hours")]
    public async Task<ActionResult<IReadOnlyList<WorkedHoursDto>>> GetCaregiverHours(
        int caregiverId,
        [FromQuery] DateOnly fromDate,
        [FromQuery] DateOnly toDate,
        CancellationToken cancellationToken = default)
    {
        var result = await _reportingAppService.GetCaregiverHoursAsync(caregiverId, fromDate, toDate, cancellationToken);
        return Ok(result);
    }
}
