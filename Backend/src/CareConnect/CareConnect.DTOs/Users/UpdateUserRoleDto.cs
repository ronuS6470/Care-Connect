using CareConnect.DTOs.Enums;

namespace CareConnect.DTOs.Users;

public sealed class UpdateUserRoleDto
{
    public required UserRole Role { get; init; }
}
