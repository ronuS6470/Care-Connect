using CareConnect.DTOs.Reporting;
using CareConnect.Queries.Reporting.Repositories;
using MediatR;

namespace CareConnect.Queries.Reporting;

public sealed class GetCaregiverHoursQueryHandler : IRequestHandler<GetCaregiverHoursQuery, IReadOnlyList<WorkedHoursDto>>
{
    private readonly ICaregiverEarningsReadRepository _repository;

    public GetCaregiverHoursQueryHandler(ICaregiverEarningsReadRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<WorkedHoursDto>> Handle(GetCaregiverHoursQuery request, CancellationToken cancellationToken) =>
        _repository.GetHoursByDateAsync(
            request.CaregiverId,
            request.FromDate,
            request.ToDate,
            request.RequestingAuth0UserId,
            cancellationToken);
}
