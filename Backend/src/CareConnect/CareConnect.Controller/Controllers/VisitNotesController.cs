using CareConnect.Commands.Visits;
using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Errors;
using CareConnect.DTOs.Visits;
using CareConnect.Queries.Visits;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareConnect.Controller.Controllers;

[ApiController]
[Route("api/visit-notes")]
[Authorize]
public sealed class VisitNotesController : ControllerBase
{
    private const string AdminOrCaregiverRoles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Caregiver)}";

    private readonly IMediator _mediator;

    public VisitNotesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private string RequestingAuth0UserId =>
        User.FindFirst("sub")?.Value
            ?? throw new ForbiddenException("Token is missing a subject claim.");

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<VisitNoteDto>>> GetVisitNotes(
        [FromQuery] int visitId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetVisitNotesQuery(visitId, RequestingAuth0UserId), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = AdminOrCaregiverRoles)]
    public async Task<ActionResult<int>> AddVisitNote([FromBody] CreateVisitNoteDto dto, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(new AddVisitNoteCommand(dto, RequestingAuth0UserId), cancellationToken);
        return CreatedAtAction(nameof(GetVisitNotes), new { visitId = dto.VisitId }, id);
    }
}
