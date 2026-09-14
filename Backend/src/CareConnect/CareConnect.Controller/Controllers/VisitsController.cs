using CareConnect.AppServices.Visits;
using CareConnect.DTOs.Common;
using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Reporting;
using CareConnect.DTOs.Visits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareConnect.Controller.Controllers;

[ApiController]
[Route("api/visits")]
[Authorize]
public sealed class VisitsController : ControllerBase
{
    private const string AdminOrCaregiverRoles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Caregiver)}";

    private readonly IVisitsAppService _visitsAppService;

    public VisitsController(IVisitsAppService visitsAppService)
    {
        _visitsAppService = visitsAppService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponseDto<VisitSummaryDto>>> GetVisits(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null,
        [FromQuery] int? caregiverId = null,
        [FromQuery] int? clientId = null,
        [FromQuery] VisitStatus? status = null,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _visitsAppService.GetVisitsAsync(
            page, pageSize, fromDate, toDate, caregiverId, clientId, status, search, cancellationToken);
        return Ok(result);
    }

    [HttpGet("today")]
    public async Task<ActionResult<IReadOnlyList<VisitSummaryDto>>> GetTodaysVisits(CancellationToken cancellationToken)
    {
        var result = await _visitsAppService.GetTodaysVisitsAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("upcoming")]
    public async Task<ActionResult<PagedResponseDto<VisitSummaryDto>>> GetUpcomingVisits(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _visitsAppService.GetUpcomingVisitsAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VisitDto>> GetVisitById(int id, CancellationToken cancellationToken)
    {
        var result = await _visitsAppService.GetVisitByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<int>> CreateVisit([FromBody] CreateVisitDto dto, CancellationToken cancellationToken)
    {
        var id = await _visitsAppService.CreateVisitAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetVisitById), new { id }, id);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> UpdateVisit(int id, [FromBody] UpdateVisitDto dto, CancellationToken cancellationToken)
    {
        await _visitsAppService.UpdateVisitAsync(id, dto, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/cancel")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> CancelVisit(int id, [FromBody] CancelVisitDto dto, CancellationToken cancellationToken)
    {
        await _visitsAppService.CancelVisitAsync(id, dto, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/check-in")]
    [Authorize(Roles = nameof(UserRole.Caregiver))]
    public async Task<IActionResult> CheckIn(int id, CancellationToken cancellationToken)
    {
        await _visitsAppService.CheckInAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/check-out")]
    [Authorize(Roles = nameof(UserRole.Caregiver))]
    public async Task<IActionResult> CheckOut(int id, CancellationToken cancellationToken)
    {
        await _visitsAppService.CheckOutAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/complete")]
    [Authorize(Roles = nameof(UserRole.Caregiver))]
    public async Task<IActionResult> Complete(int id, [FromBody] CompleteVisitDto dto, CancellationToken cancellationToken)
    {
        await _visitsAppService.CompleteAsync(id, dto, cancellationToken);
        return NoContent();
    }

    [HttpGet("{visitId:int}/tasks")]
    public async Task<ActionResult<IReadOnlyList<VisitTaskDto>>> GetVisitTasks(int visitId, CancellationToken cancellationToken)
    {
        var result = await _visitsAppService.GetVisitTasksAsync(visitId, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{visitId:int}/tasks/{taskId:int}")]
    [Authorize(Roles = AdminOrCaregiverRoles)]
    public async Task<IActionResult> UpdateVisitTask(
        int visitId,
        int taskId,
        [FromBody] UpdateVisitTaskDto dto,
        CancellationToken cancellationToken)
    {
        await _visitsAppService.UpdateVisitTaskAsync(visitId, taskId, dto, cancellationToken);
        return NoContent();
    }

    [HttpPost("{visitId:int}/tasks/{taskId:int}/complete")]
    [Authorize(Roles = AdminOrCaregiverRoles)]
    public async Task<IActionResult> CompleteVisitTask(int visitId, int taskId, CancellationToken cancellationToken)
    {
        await _visitsAppService.CompleteVisitTaskAsync(visitId, taskId, cancellationToken);
        return NoContent();
    }

    [HttpPost("{visitId:int}/tasks/{taskId:int}/uncomplete")]
    [Authorize(Roles = AdminOrCaregiverRoles)]
    public async Task<IActionResult> UncompleteVisitTask(int visitId, int taskId, CancellationToken cancellationToken)
    {
        await _visitsAppService.UncompleteVisitTaskAsync(visitId, taskId, cancellationToken);
        return NoContent();
    }
}
