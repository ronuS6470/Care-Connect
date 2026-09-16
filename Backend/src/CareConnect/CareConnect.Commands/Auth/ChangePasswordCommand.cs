using CareConnect.DTOs.Auth;
using MediatR;

namespace CareConnect.Commands.Auth;

/// <summary>A signed-in user changing their own password. The target is always the caller.</summary>
public sealed record ChangePasswordCommand(ChangePasswordDto Password, string RequestingAuth0UserId) : IRequest;
