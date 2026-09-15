using CareConnect.DTOs.Reporting;
using CareConnect.Queries.Reporting.Repositories;
using MediatR;

namespace CareConnect.Queries.Reporting;

public sealed class GetCaregiverEarningsQueryHandler : IRequestHandler<GetCaregiverEarningsQuery, CaregiverEarningsDto>
{
    private readonly ICaregiverEarningsReadRepository _repository;

    public GetCaregiverEarningsQueryHandler(ICaregiverEarningsReadRepository repository)
    {
        _repository = repository;
    }

    public Task<CaregiverEarningsDto> Handle(GetCaregiverEarningsQuery request, CancellationToken cancellationToken) =>
        _repository.GetEarningsAsync(
            request.CaregiverId,
            request.FromDate,
            request.ToDate,
            request.IncludeVisitBreakdown,
            request.RequestingAuth0UserId,
            cancellationToken);
}
