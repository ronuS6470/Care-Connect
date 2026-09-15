using CareConnect.DTOs.Auth;
using MediatR;

namespace CareConnect.Commands.Auth;

/// <summary>Verifies credentials and, on success, issues an access token for that user.</summary>
public sealed record LoginCommand(LoginRequestDto Credentials) : IRequest<LoginResponseDto>;
