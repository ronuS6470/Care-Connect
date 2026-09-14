using CareConnect.DTOs.Caregivers;
using CareConnect.Queries.Availability.Repositories;
using MediatR;

namespace CareConnect.Queries.Availability;

public sealed class GetCaregiverAvailabilityQueryHandler
    : IRequestHandler<GetCaregiverAvailabilityQuery, IReadOnlyList<CaregiverAvailabilityDto>>
{
    private readonly ICaregiverAvailabilityReadRepository _repository;

    public GetCaregiverAvailabilityQueryHandler(ICaregiverAvailabilityReadRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<CaregiverAvailabilityDto>> Handle(
        GetCaregiverAvailabilityQuery request,
        CancellationToken cancellationToken) =>
        _repository.GetByCaregiverIdAsync(request.CaregiverId, cancellationToken);
}
