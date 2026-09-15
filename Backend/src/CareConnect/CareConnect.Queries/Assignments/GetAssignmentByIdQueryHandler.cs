using CareConnect.DTOs.Assignments;
using CareConnect.Queries.Assignments.Repositories;
using MediatR;

namespace CareConnect.Queries.Assignments;

public sealed class GetAssignmentByIdQueryHandler : IRequestHandler<GetAssignmentByIdQuery, AssignmentDto?>
{
    private readonly IAssignmentReadRepository _repository;

    public GetAssignmentByIdQueryHandler(IAssignmentReadRepository repository)
    {
        _repository = repository;
    }

    public Task<AssignmentDto?> Handle(GetAssignmentByIdQuery request, CancellationToken cancellationToken) =>
        _repository.GetByIdAsync(request.AssignmentId, request.RequestingAuth0UserId, cancellationToken);
}
