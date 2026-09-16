using MediatR;

namespace CareConnect.Commands.Users;

/// <summary>Admin-only: enables or disables an account. A disabled account cannot sign in.</summary>
public sealed record SetUserActiveCommand(int UserId, bool IsActive, string RequestingAuth0UserId) : IRequest;
