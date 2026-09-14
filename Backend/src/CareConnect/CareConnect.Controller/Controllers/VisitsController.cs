using CareConnect.Commands.Visits;
using CareConnect.DTOs.Common;
using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Errors;
using CareConnect.DTOs.Reporting;
using CareConnect.DTOs.Visits;
using CareConnect.Queries.Visits;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareConnect.Controller.Controllers;

[ApiController]
[Route("api/visits")]
[Authorize]
public sealed class VisitsController : ControllerBase
{
    private readonly IMediator _mediator;

    public VisitsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private const string AdminOrCaregiverRoles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Caregiver)}";

    private string RequestingAuth0UserId =>
        User.FindFirst("sub")?.Value
            ?? throw new ForbiddenException("Token is missing a subject claim.");

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
        var result = await _mediator.Send(
            new GetVisitsQuery(page, pageSize, fromDate, toDate, caregiverId, clientId, status, search, RequestingAuth0UserId),
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("today")]
    public async Task<ActionResult<IReadOnlyList<VisitSummaryDto>>> GetTodaysVisits(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTodaysVisitsQuery(RequestingAuth0UserId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("upcoming")]
    public async Task<ActionResult<PagedResponseDto<VisitSummaryDto>>> GetUpcomingVisits(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetUpcomingVisitsQuery(page, pageSize, RequestingAuth0UserId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VisitDto>> GetVisitById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetVisitByIdQuery(id, RequestingAuth0UserId), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<int>> CreateVisit([FromBody] CreateVisitDto dto, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(new CreateVisitCommand(dto), cancellationToken);
        return CreatedAtAction(nameof(GetVisitById), new { id }, id);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> UpdateVisit(int id, [FromBody] UpdateVisitDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateVisitCommand(id, dto), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/cancel")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> CancelVisit(int id, [FromBody] CancelVisitDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new CancelVisitCommand(id, dto), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/check-in")]
    [Authorize(Roles = nameof(UserRole.Caregiver))]
    public async Task<IActionResult> CheckIn(int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new CheckInVisitCommand(id, RequestingAuth0UserId), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/check-out")]
    [Authorize(Roles = nameof(UserRole.Caregiver))]
    public async Task<IActionResult> CheckOut(int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new CheckOutVisitCommand(id, RequestingAuth0UserId), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/complete")]
    [Authorize(Roles = nameof(UserRole.Caregiver))]
    public async Task<IActionResult> Complete(int id, [FromBody] CompleteVisitDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new CompleteVisitCommand(id, RequestingAuth0UserId, dto), cancellationToken);
        return NoContent();
    }

    [HttpGet("{visitId:int}/tasks")]
    public async Task<ActionResult<IReadOnlyList<VisitTaskDto>>> GetVisitTasks(int visitId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetVisitTasksQuery(visitId, RequestingAuth0UserId), cancellationToken);
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
        await _mediator.Send(new UpdateVisitTaskCommand(visitId, taskId, dto, RequestingAuth0UserId), cancellationToken);
        return NoContent();
    }

    [HttpPost("{visitId:int}/tasks/{taskId:int}/complete")]
    [Authorize(Roles = AdminOrCaregiverRoles)]
    public async Task<IActionResult> CompleteVisitTask(int visitId, int taskId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new CompleteVisitTaskCommand(visitId, taskId, RequestingAuth0UserId), cancellationToken);
        return NoContent();
    }

    [HttpPost("{visitId:int}/tasks/{taskId:int}/uncomplete")]
    [Authorize(Roles = AdminOrCaregiverRoles)]
    public async Task<IActionResult> UncompleteVisitTask(int visitId, int taskId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UncompleteVisitTaskCommand(visitId, taskId, RequestingAuth0UserId), cancellationToken);
        return NoContent();
    }
}
