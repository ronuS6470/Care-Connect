using CareConnect.DTOs.Visits;
using CareConnect.Queries.Visits.Repositories;
using MediatR;

namespace CareConnect.Queries.Visits;

public sealed class GetVisitNotesQueryHandler : IRequestHandler<GetVisitNotesQuery, IReadOnlyList<VisitNoteDto>>
{
    private readonly IVisitReadRepository _repository;

    public GetVisitNotesQueryHandler(IVisitReadRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<VisitNoteDto>> Handle(GetVisitNotesQuery request, CancellationToken cancellationToken) =>
        _repository.GetNotesAsync(request.VisitId, request.RequestingAuth0UserId, cancellationToken);
}
