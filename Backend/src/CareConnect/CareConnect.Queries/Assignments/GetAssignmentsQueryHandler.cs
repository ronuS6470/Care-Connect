using CareConnect.DTOs.Assignments;
using CareConnect.DTOs.Common;
using CareConnect.Queries.Assignments.Repositories;
using MediatR;

namespace CareConnect.Queries.Assignments;

public sealed class GetAssignmentsQueryHandler : IRequestHandler<GetAssignmentsQuery, PagedResponseDto<AssignmentDto>>
{
    private readonly IAssignmentReadRepository _repository;

    public GetAssignmentsQueryHandler(IAssignmentReadRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResponseDto<AssignmentDto>> Handle(GetAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        return _repository.GetPagedAsync(page, pageSize, request.Status, request.RequestingAuth0UserId, cancellationToken);
    }
}
