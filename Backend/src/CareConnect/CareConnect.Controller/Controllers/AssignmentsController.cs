using CareConnect.Commands.Assignments;
using CareConnect.DTOs.Assignments;
using CareConnect.DTOs.Common;
using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Errors;
using CareConnect.Queries.Assignments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareConnect.Controller.Controllers;

[ApiController]
[Route("api/assignments")]
[Authorize]
public sealed class AssignmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AssignmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private string RequestingAuth0UserId =>
        User.FindFirst("sub")?.Value
            ?? throw new ForbiddenException("Token is missing a subject claim.");

    [HttpGet]
    public async Task<ActionResult<PagedResponseDto<AssignmentDto>>> GetAssignments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] AssignmentStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetAssignmentsQuery(page, pageSize, status, RequestingAuth0UserId),
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AssignmentDto>> GetAssignmentById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAssignmentByIdQuery(id, RequestingAuth0UserId), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<int>> CreateAssignment(
        [FromBody] CreateAssignmentDto dto,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(new CreateAssignmentCommand(dto), cancellationToken);
        return CreatedAtAction(nameof(GetAssignmentById), new { id }, id);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> UpdateAssignment(
        int id,
        [FromBody] UpdateAssignmentDto dto,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateAssignmentCommand(id, dto), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/cancel")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> CancelAssignment(int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new CancelAssignmentCommand(id), cancellationToken);
        return NoContent();
    }
}
