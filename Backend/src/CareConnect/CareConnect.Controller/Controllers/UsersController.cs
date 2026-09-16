using CareConnect.AppServices.Users;
using CareConnect.DTOs.Common;
using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareConnect.Controller.Controllers;

/// <summary>
/// Account administration: who exists, what role they hold, whether they can sign in, and password
/// resets. Admin-only in full — the Queries/Commands layers re-check rather than trusting this.
/// </summary>
[ApiController]
[Route("api/users")]
[Authorize(Roles = nameof(UserRole.Admin))]
public sealed class UsersController : ControllerBase
{
    private readonly IUsersAppService _usersAppService;

    public UsersController(IUsersAppService usersAppService)
    {
        _usersAppService = usersAppService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponseDto<UserDto>>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] UserRole? role = null,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _usersAppService.GetUsersAsync(page, pageSize, search, role, isActive, cancellationToken);
        return Ok(result);
    }

    /// <summary>Takes effect at the user's next sign-in — an access token already issued keeps its old role claim.</summary>
    [HttpPut("{id:int}/role")]
    public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleDto dto, CancellationToken cancellationToken)
    {
        await _usersAppService.UpdateUserRoleAsync(id, dto, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/activate")]
    public async Task<IActionResult> ActivateUser(int id, CancellationToken cancellationToken)
    {
        await _usersAppService.SetUserActiveAsync(id, isActive: true, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/deactivate")]
    public async Task<IActionResult> DeactivateUser(int id, CancellationToken cancellationToken)
    {
        await _usersAppService.SetUserActiveAsync(id, isActive: false, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/reset-password")]
    public async Task<IActionResult> ResetUserPassword(int id, [FromBody] ResetUserPasswordDto dto, CancellationToken cancellationToken)
    {
        await _usersAppService.ResetUserPasswordAsync(id, dto, cancellationToken);
        return NoContent();
    }
}
