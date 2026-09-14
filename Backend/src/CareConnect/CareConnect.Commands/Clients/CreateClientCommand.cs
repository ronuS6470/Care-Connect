using CareConnect.DTOs.Clients;
using MediatR;

namespace CareConnect.Commands.Clients;

/// <summary>Creates a client profile for an existing user. Returns the new client's Id.</summary>
public sealed record CreateClientCommand(CreateClientDto Client) : IRequest<int>;
