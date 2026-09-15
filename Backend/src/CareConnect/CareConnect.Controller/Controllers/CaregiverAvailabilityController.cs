using CareConnect.AppServices.CaregiverAvailability;
using CareConnect.DTOs.Caregivers;
using CareConnect.DTOs.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareConnect.Controller.Controllers;

[ApiController]
[Route("api/caregiver-availability")]
[Authorize]
public sealed class CaregiverAvailabilityController : ControllerBase
{
    private readonly ICaregiverAvailabilityAppService _caregiverAvailabilityAppService;

    public CaregiverAvailabilityController(ICaregiverAvailabilityAppService caregiverAvailabilityAppService)
    {
        _caregiverAvailabilityAppService = caregiverAvailabilityAppService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CaregiverAvailabilityDto>>> GetCaregiverAvailability(
        [FromQuery] int caregiverId,
        CancellationToken cancellationToken)
    {
        var result = await _caregiverAvailabilityAppService.GetCaregiverAvailabilityAsync(caregiverId, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<int>> CreateAvailability(
        [FromBody] CreateCaregiverAvailabilityDto dto,
        CancellationToken cancellationToken)
    {
        var id = await _caregiverAvailabilityAppService.CreateAvailabilityAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetCaregiverAvailability), new { caregiverId = dto.CaregiverId }, id);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> UpdateAvailability(
        int id,
        [FromBody] UpdateCaregiverAvailabilityDto dto,
        CancellationToken cancellationToken)
    {
        await _caregiverAvailabilityAppService.UpdateAvailabilityAsync(id, dto, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> DeleteAvailability(int id, CancellationToken cancellationToken)
    {
        await _caregiverAvailabilityAppService.DeleteAvailabilityAsync(id, cancellationToken);
        return NoContent();
    }
}
