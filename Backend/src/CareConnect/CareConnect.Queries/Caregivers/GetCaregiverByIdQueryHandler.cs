using CareConnect.DTOs.Caregivers;
using CareConnect.Queries.Caregivers.Repositories;
using MediatR;

namespace CareConnect.Queries.Caregivers;

public sealed class GetCaregiverByIdQueryHandler : IRequestHandler<GetCaregiverByIdQuery, CaregiverDto?>
{
    private readonly ICaregiverReadRepository _repository;

    public GetCaregiverByIdQueryHandler(ICaregiverReadRepository repository)
    {
        _repository = repository;
    }

    public Task<CaregiverDto?> Handle(GetCaregiverByIdQuery request, CancellationToken cancellationToken) =>
        _repository.GetByIdAsync(request.CaregiverId, cancellationToken);
}
