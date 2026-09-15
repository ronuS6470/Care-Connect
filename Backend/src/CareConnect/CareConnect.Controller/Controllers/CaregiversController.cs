using CareConnect.AppServices.Caregivers;
using CareConnect.DTOs.Caregivers;
using CareConnect.DTOs.Common;
using CareConnect.DTOs.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareConnect.Controller.Controllers;

[ApiController]
[Route("api/caregivers")]
[Authorize]
public sealed class CaregiversController : ControllerBase
{
    private readonly ICaregiversAppService _caregiversAppService;

    public CaregiversController(ICaregiversAppService caregiversAppService)
    {
        _caregiversAppService = caregiversAppService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponseDto<CaregiverDto>>> GetCaregivers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _caregiversAppService.GetCaregiversAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CaregiverDto>> GetCaregiverById(int id, CancellationToken cancellationToken)
    {
        var result = await _caregiversAppService.GetCaregiverByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<int>> CreateCaregiver(
        [FromBody] CreateCaregiverDto dto,
        CancellationToken cancellationToken)
    {
        var id = await _caregiversAppService.CreateCaregiverAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetCaregiverById), new { id }, id);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> UpdateCaregiver(
        int id,
        [FromBody] UpdateCaregiverDto dto,
        CancellationToken cancellationToken)
    {
        await _caregiversAppService.UpdateCaregiverAsync(id, dto, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> DeleteCaregiver(int id, CancellationToken cancellationToken)
    {
        await _caregiversAppService.DeleteCaregiverAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/activate")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> ActivateCaregiver(int id, CancellationToken cancellationToken)
    {
        await _caregiversAppService.ActivateCaregiverAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/deactivate")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> DeactivateCaregiver(int id, CancellationToken cancellationToken)
    {
        await _caregiversAppService.DeactivateCaregiverAsync(id, cancellationToken);
        return NoContent();
    }
}
