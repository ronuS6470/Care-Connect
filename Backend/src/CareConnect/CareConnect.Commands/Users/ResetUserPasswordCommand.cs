using CareConnect.DTOs.Users;
using MediatR;

namespace CareConnect.Commands.Users;

/// <summary>Admin-only: sets another account's password without knowing the current one.</summary>
public sealed record ResetUserPasswordCommand(int UserId, ResetUserPasswordDto Password) : IRequest;
