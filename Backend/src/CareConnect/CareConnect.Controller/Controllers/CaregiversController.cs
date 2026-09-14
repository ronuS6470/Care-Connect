using CareConnect.Commands.Caregivers;
using CareConnect.DTOs.Caregivers;
using CareConnect.DTOs.Common;
using CareConnect.DTOs.Enums;
using CareConnect.Queries.Caregivers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareConnect.Controller.Controllers;

[ApiController]
[Route("api/caregivers")]
[Authorize]
public sealed class CaregiversController : ControllerBase
{
    private readonly IMediator _mediator;

    public CaregiversController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponseDto<CaregiverDto>>> GetCaregivers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetCaregiversQuery(page, pageSize), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CaregiverDto>> GetCaregiverById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCaregiverByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<int>> CreateCaregiver(
        [FromBody] CreateCaregiverDto dto,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(new CreateCaregiverCommand(dto), cancellationToken);
        return CreatedAtAction(nameof(GetCaregiverById), new { id }, id);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> UpdateCaregiver(
        int id,
        [FromBody] UpdateCaregiverDto dto,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateCaregiverCommand(id, dto), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> DeleteCaregiver(int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCaregiverCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/activate")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> ActivateCaregiver(int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new ActivateCaregiverCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/deactivate")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> DeactivateCaregiver(int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeactivateCaregiverCommand(id), cancellationToken);
        return NoContent();
    }
}
