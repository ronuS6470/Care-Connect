using CareConnect.DTOs.CareTasks;
using CareConnect.DTOs.Common;
using CareConnect.Queries.CareTasks.Repositories;
using MediatR;

namespace CareConnect.Queries.CareTasks;

public sealed class GetCareTasksQueryHandler : IRequestHandler<GetCareTasksQuery, PagedResponseDto<CareTaskDto>>
{
    private readonly ICareTaskReadRepository _repository;

    public GetCareTasksQueryHandler(ICareTaskReadRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResponseDto<CareTaskDto>> Handle(GetCareTasksQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 200);

        return _repository.GetPagedAsync(page, pageSize, request.IsActive, cancellationToken);
    }
}
