using CareConnect.DTOs.Reports;
using CareConnect.Queries.Reports.Repositories;
using MediatR;

namespace CareConnect.Queries.Reports;

public sealed class GetClientsWithoutActiveCaregiverReportQueryHandler
    : IRequestHandler<GetClientsWithoutActiveCaregiverReportQuery, IReadOnlyList<ClientWithoutActiveCaregiverDto>>
{
    private readonly IReportsReadRepository _repository;

    public GetClientsWithoutActiveCaregiverReportQueryHandler(IReportsReadRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<ClientWithoutActiveCaregiverDto>> Handle(
        GetClientsWithoutActiveCaregiverReportQuery request, CancellationToken cancellationToken) =>
        _repository.GetClientsWithoutActiveCaregiverAsync(request.RequestingAuth0UserId, cancellationToken);
}
