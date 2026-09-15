using CareConnect.AppServices.Dashboards;
using CareConnect.DTOs.Dashboards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareConnect.Controller.Controllers;

/// <summary>
/// Each endpoint is always "my dashboard" — role-restricted via [Authorize(Roles=...)] and, in the
/// handler, re-verified server-side against the caller's real DB role (RequesterResolver), not just
/// the JWT's role claim. None of these accept an id from the caller.
/// </summary>
[ApiController]
[Route("api/dashboard")]
[Authorize]
public sealed class DashboardController : ControllerBase
{
    private readonly IDashboardAppService _dashboardAppService;

    public DashboardController(IDashboardAppService dashboardAppService)
    {
        _dashboardAppService = dashboardAppService;
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AdminDashboardDto>> GetAdminDashboard(CancellationToken cancellationToken)
    {
        var result = await _dashboardAppService.GetAdminDashboardAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("caregiver")]
    [Authorize(Roles = "Caregiver")]
    public async Task<ActionResult<CaregiverDashboardDto>> GetCaregiverDashboard(CancellationToken cancellationToken)
    {
        var result = await _dashboardAppService.GetCaregiverDashboardAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("client")]
    [Authorize(Roles = "Client")]
    public async Task<ActionResult<ClientDashboardDto>> GetClientDashboard(CancellationToken cancellationToken)
    {
        var result = await _dashboardAppService.GetClientDashboardAsync(cancellationToken);
        return Ok(result);
    }
}
