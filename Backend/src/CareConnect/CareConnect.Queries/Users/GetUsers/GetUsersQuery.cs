using CareConnect.DTOs.Common;
using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Users;
using MediatR;

namespace CareConnect.Queries.Users.GetUsers;

/// <summary>Admin-only account listing for user management.</summary>
public sealed record GetUsersQuery(
    int Page,
    int PageSize,
    string? Search,
    UserRole? Role,
    bool? IsActive,
    string RequestingAuth0UserId) : IRequest<PagedResponseDto<UserDto>>;
