using CareConnect.DTOs.Caregivers;
using CareConnect.DTOs.Common;
using CareConnect.Queries.Caregivers.Repositories;
using MediatR;

namespace CareConnect.Queries.Caregivers;

public sealed class GetCaregiversQueryHandler : IRequestHandler<GetCaregiversQuery, PagedResponseDto<CaregiverDto>>
{
    private readonly ICaregiverReadRepository _repository;

    public GetCaregiversQueryHandler(ICaregiverReadRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResponseDto<CaregiverDto>> Handle(GetCaregiversQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        return _repository.GetPagedAsync(page, pageSize, cancellationToken);
    }
}
