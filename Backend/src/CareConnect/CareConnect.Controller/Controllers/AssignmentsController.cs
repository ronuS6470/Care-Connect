using CareConnect.AppServices.Assignments;
using CareConnect.DTOs.Assignments;
using CareConnect.DTOs.Common;
using CareConnect.DTOs.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareConnect.Controller.Controllers;

[ApiController]
[Route("api/assignments")]
[Authorize]
public sealed class AssignmentsController : ControllerBase
{
    private readonly IAssignmentsAppService _assignmentsAppService;

    public AssignmentsController(IAssignmentsAppService assignmentsAppService)
    {
        _assignmentsAppService = assignmentsAppService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponseDto<AssignmentDto>>> GetAssignments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] AssignmentStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _assignmentsAppService.GetAssignmentsAsync(page, pageSize, status, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AssignmentDto>> GetAssignmentById(int id, CancellationToken cancellationToken)
    {
        var result = await _assignmentsAppService.GetAssignmentByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<int>> CreateAssignment(
        [FromBody] CreateAssignmentDto dto,
        CancellationToken cancellationToken)
    {
        var id = await _assignmentsAppService.CreateAssignmentAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetAssignmentById), new { id }, id);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> UpdateAssignment(
        int id,
        [FromBody] UpdateAssignmentDto dto,
        CancellationToken cancellationToken)
    {
        await _assignmentsAppService.UpdateAssignmentAsync(id, dto, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/cancel")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> CancelAssignment(int id, CancellationToken cancellationToken)
    {
        await _assignmentsAppService.CancelAssignmentAsync(id, cancellationToken);
        return NoContent();
    }
}
