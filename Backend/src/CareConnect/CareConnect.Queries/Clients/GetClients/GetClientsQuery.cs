using CareConnect.DTOs.Clients;
using CareConnect.DTOs.Common;
using MediatR;

namespace CareConnect.Queries.Clients.GetClients;

public sealed record GetClientsQuery(
    int Page,
    int PageSize,
    string? Search,
    bool? IsActive,
    string RequestingAuth0UserId) : IRequest<PagedResponseDto<ClientDto>>;
