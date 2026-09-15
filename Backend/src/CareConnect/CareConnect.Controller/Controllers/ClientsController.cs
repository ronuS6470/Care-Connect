using CareConnect.AppServices.Clients;
using CareConnect.DTOs.Clients;
using CareConnect.DTOs.Common;
using CareConnect.DTOs.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareConnect.Controller.Controllers;

[ApiController]
[Route("api/clients")]
[Authorize]
public sealed class ClientsController : ControllerBase
{
    private readonly IClientsAppService _clientsAppService;

    public ClientsController(IClientsAppService clientsAppService)
    {
        _clientsAppService = clientsAppService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponseDto<ClientDto>>> GetClients(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _clientsAppService.GetClientsAsync(page, pageSize, search, isActive, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClientDto>> GetClientById(int id, CancellationToken cancellationToken)
    {
        var result = await _clientsAppService.GetClientByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<int>> CreateClient(
        [FromBody] CreateClientDto dto,
        CancellationToken cancellationToken)
    {
        var id = await _clientsAppService.CreateClientAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetClientById), new { id }, id);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> UpdateClient(
        int id,
        [FromBody] UpdateClientDto dto,
        CancellationToken cancellationToken)
    {
        await _clientsAppService.UpdateClientAsync(id, dto, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> DeleteClient(int id, CancellationToken cancellationToken)
    {
        await _clientsAppService.DeleteClientAsync(id, cancellationToken);
        return NoContent();
    }
}
