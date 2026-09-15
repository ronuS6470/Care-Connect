using CareConnect.Commands.Auth;
using CareConnect.DTOs.Auth;
using MediatR;

namespace CareConnect.AppServices.Auth;

public sealed class AuthAppService : IAuthAppService
{
    private readonly IMediator _mediator;

    public AuthAppService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<LoginResponseDto> LoginAsync(LoginRequestDto credentials, CancellationToken cancellationToken) =>
        _mediator.Send(new LoginCommand(credentials), cancellationToken);
}
