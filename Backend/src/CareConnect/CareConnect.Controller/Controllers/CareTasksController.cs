using CareConnect.Commands.CareTasks;
using CareConnect.DTOs.CareTasks;
using CareConnect.DTOs.Common;
using CareConnect.DTOs.Enums;
using CareConnect.Queries.CareTasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareConnect.Controller.Controllers;

[ApiController]
[Route("api/care-tasks")]
[Authorize]
public sealed class CareTasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public CareTasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponseDto<CareTaskDto>>> GetCareTasks(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetCareTasksQuery(page, pageSize, isActive), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CareTaskDto>> GetCareTaskById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCareTaskByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<int>> CreateCareTask(
        [FromBody] CreateCareTaskDto dto,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(new CreateCareTaskCommand(dto), cancellationToken);
        return CreatedAtAction(nameof(GetCareTaskById), new { id }, id);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> UpdateCareTask(
        int id,
        [FromBody] UpdateCareTaskDto dto,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateCareTaskCommand(id, dto), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> DeleteCareTask(int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCareTaskCommand(id), cancellationToken);
        return NoContent();
    }
}
