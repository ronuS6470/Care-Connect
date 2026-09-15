using CareConnect.AppServices.Auth;
using CareConnect.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareConnect.Controller.Controllers;

/// <summary>
/// Sign-in. Anonymous by necessity — this is where a caller gets the bearer token every other
/// controller requires.
/// </summary>
[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthAppService _authAppService;

    public AuthController(IAuthAppService authAppService)
    {
        _authAppService = authAppService;
    }

    /// <summary>Returns the access token plus the identity fields the client renders. 401 on any credential failure.</summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(
        [FromBody] LoginRequestDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _authAppService.LoginAsync(dto, cancellationToken);
        return Ok(result);
    }
}
