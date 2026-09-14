using CareConnect.DTOs.Visits;
using CareConnect.Queries.Visits.Repositories;
using MediatR;

namespace CareConnect.Queries.Visits;

public sealed class GetVisitTasksQueryHandler : IRequestHandler<GetVisitTasksQuery, IReadOnlyList<VisitTaskDto>>
{
    private readonly IVisitReadRepository _repository;

    public GetVisitTasksQueryHandler(IVisitReadRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<VisitTaskDto>> Handle(GetVisitTasksQuery request, CancellationToken cancellationToken) =>
        _repository.GetTasksAsync(request.VisitId, request.RequestingAuth0UserId, cancellationToken);
}
