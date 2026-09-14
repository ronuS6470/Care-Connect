using CareConnect.DTOs.Clients;
using MediatR;

namespace CareConnect.Queries.Clients;

public sealed record GetClientByIdQuery(int ClientId, string RequestingAuth0UserId) : IRequest<ClientDto?>;
