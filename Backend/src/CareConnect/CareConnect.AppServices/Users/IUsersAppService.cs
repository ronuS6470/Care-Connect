using CareConnect.DTOs.Common;
using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Users;

namespace CareConnect.AppServices.Users;

public interface IUsersAppService
{
    Task<PagedResponseDto<UserDto>> GetUsersAsync(
        int page, int pageSize, string? search, UserRole? role, bool? isActive, CancellationToken cancellationToken);

    Task UpdateUserRoleAsync(int userId, UpdateUserRoleDto dto, CancellationToken cancellationToken);

    Task SetUserActiveAsync(int userId, bool isActive, CancellationToken cancellationToken);

    Task ResetUserPasswordAsync(int userId, ResetUserPasswordDto dto, CancellationToken cancellationToken);
}
