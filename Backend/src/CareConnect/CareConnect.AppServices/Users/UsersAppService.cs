using CareConnect.AppServices.Security;
using CareConnect.Commands.Users;
using CareConnect.DTOs.Common;
using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Users;
using CareConnect.Queries.Users.GetUsers;
using MediatR;

namespace CareConnect.AppServices.Users;

public sealed class UsersAppService : IUsersAppService
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public UsersAppService(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    {
        _mediator = mediator;
        _currentUserAccessor = currentUserAccessor;
    }

    public Task<PagedResponseDto<UserDto>> GetUsersAsync(
        int page, int pageSize, string? search, UserRole? role, bool? isActive, CancellationToken cancellationToken) =>
        _mediator.Send(new GetUsersQuery(page, pageSize, search, role, isActive, _currentUserAccessor.Auth0UserId), cancellationToken);

    public Task UpdateUserRoleAsync(int userId, UpdateUserRoleDto dto, CancellationToken cancellationToken) =>
        _mediator.Send(new UpdateUserRoleCommand(userId, dto, _currentUserAccessor.Auth0UserId), cancellationToken);

    public Task SetUserActiveAsync(int userId, bool isActive, CancellationToken cancellationToken) =>
        _mediator.Send(new SetUserActiveCommand(userId, isActive, _currentUserAccessor.Auth0UserId), cancellationToken);

    public Task ResetUserPasswordAsync(int userId, ResetUserPasswordDto dto, CancellationToken cancellationToken) =>
        _mediator.Send(new ResetUserPasswordCommand(userId, dto), cancellationToken);
}
