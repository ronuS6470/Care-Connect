using CareConnect.DTOs.Clients;
using MediatR;

namespace CareConnect.Commands.Clients;

public sealed record UpdateClientCommand(int ClientId, UpdateClientDto Client) : IRequest;
