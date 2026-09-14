using CareConnect.DTOs.Visits;
using CareConnect.Queries.Visits.Repositories;
using MediatR;

namespace CareConnect.Queries.Visits;

public sealed class GetVisitByIdQueryHandler : IRequestHandler<GetVisitByIdQuery, VisitDto?>
{
    private readonly IVisitReadRepository _repository;

    public GetVisitByIdQueryHandler(IVisitReadRepository repository)
    {
        _repository = repository;
    }

    public Task<VisitDto?> Handle(GetVisitByIdQuery request, CancellationToken cancellationToken) =>
        _repository.GetByIdAsync(request.VisitId, request.RequestingAuth0UserId, cancellationToken);
}
