using CareConnect.DTOs.Clients;
using MediatR;

namespace CareConnect.Queries.Clients.GetClientById;

public sealed record GetClientByIdQuery(int ClientId, string RequestingAuth0UserId) : IRequest<ClientDto?>;
