using CareConnect.Commands.Clients;
using CareConnect.DTOs.Clients;
using CareConnect.DTOs.Common;
using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Errors;
using CareConnect.Queries.Clients;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareConnect.Controller.Controllers;

[ApiController]
[Route("api/clients")]
[Authorize]
public sealed class ClientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private string RequestingAuth0UserId =>
        User.FindFirst("sub")?.Value
            ?? throw new ForbiddenException("Token is missing a subject claim.");

    [HttpGet]
    public async Task<ActionResult<PagedResponseDto<ClientDto>>> GetClients(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetClientsQuery(page, pageSize, search, isActive, RequestingAuth0UserId),
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClientDto>> GetClientById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetClientByIdQuery(id, RequestingAuth0UserId), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<int>> CreateClient(
        [FromBody] CreateClientDto dto,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(new CreateClientCommand(dto), cancellationToken);
        return CreatedAtAction(nameof(GetClientById), new { id }, id);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> UpdateClient(
        int id,
        [FromBody] UpdateClientDto dto,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateClientCommand(id, dto), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> DeleteClient(int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteClientCommand(id), cancellationToken);
        return NoContent();
    }
}
