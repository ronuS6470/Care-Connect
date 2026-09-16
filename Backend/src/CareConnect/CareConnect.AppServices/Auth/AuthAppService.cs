using CareConnect.AppServices.Security;
using CareConnect.Commands.Auth;
using CareConnect.DTOs.Auth;
using MediatR;

namespace CareConnect.AppServices.Auth;

public sealed class AuthAppService : IAuthAppService
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public AuthAppService(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    {
        _mediator = mediator;
        _currentUserAccessor = currentUserAccessor;
    }

    public Task<LoginResponseDto> LoginAsync(LoginRequestDto credentials, CancellationToken cancellationToken) =>
        _mediator.Send(new LoginCommand(credentials), cancellationToken);

    public Task ChangePasswordAsync(ChangePasswordDto dto, CancellationToken cancellationToken) =>
        _mediator.Send(new ChangePasswordCommand(dto, _currentUserAccessor.Auth0UserId), cancellationToken);
}
