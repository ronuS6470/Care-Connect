using CareConnect.AppServices.Visits;
using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Visits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareConnect.Controller.Controllers;

[ApiController]
[Route("api/visit-notes")]
[Authorize]
public sealed class VisitNotesController : ControllerBase
{
    private const string AdminOrCaregiverRoles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Caregiver)}";

    private readonly IVisitNotesAppService _visitNotesAppService;

    public VisitNotesController(IVisitNotesAppService visitNotesAppService)
    {
        _visitNotesAppService = visitNotesAppService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<VisitNoteDto>>> GetVisitNotes(
        [FromQuery] int visitId,
        CancellationToken cancellationToken)
    {
        var result = await _visitNotesAppService.GetVisitNotesAsync(visitId, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = AdminOrCaregiverRoles)]
    public async Task<ActionResult<int>> AddVisitNote([FromBody] CreateVisitNoteDto dto, CancellationToken cancellationToken)
    {
        var id = await _visitNotesAppService.AddVisitNoteAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetVisitNotes), new { visitId = dto.VisitId }, id);
    }
}
