using CareConnect.DTOs.Clients;
using CareConnect.DTOs.Common;
using CareConnect.Queries.Clients.Repositories;
using MediatR;

namespace CareConnect.Queries.Clients;

public sealed class GetClientsQueryHandler : IRequestHandler<GetClientsQuery, PagedResponseDto<ClientDto>>
{
    private readonly IClientReadRepository _repository;

    public GetClientsQueryHandler(IClientReadRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResponseDto<ClientDto>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        return _repository.GetPagedAsync(page, pageSize, request.Search, request.IsActive, request.RequestingAuth0UserId, cancellationToken);
    }
}
