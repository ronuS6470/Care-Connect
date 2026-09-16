using CareConnect.DTOs.Users;
using MediatR;

namespace CareConnect.Commands.Users;

/// <summary>Admin-only: moves an account between Admin / Caregiver / Client.</summary>
public sealed record UpdateUserRoleCommand(int UserId, UpdateUserRoleDto Role, string RequestingAuth0UserId) : IRequest;
