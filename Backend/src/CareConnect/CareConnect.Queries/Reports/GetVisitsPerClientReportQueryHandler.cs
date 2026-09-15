using CareConnect.DTOs.Reports;
using CareConnect.Queries.Reports.Repositories;
using MediatR;

namespace CareConnect.Queries.Reports;

public sealed class GetVisitsPerClientReportQueryHandler
    : IRequestHandler<GetVisitsPerClientReportQuery, IReadOnlyList<VisitsPerClientReportRowDto>>
{
    private readonly IReportsReadRepository _repository;

    public GetVisitsPerClientReportQueryHandler(IReportsReadRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<VisitsPerClientReportRowDto>> Handle(GetVisitsPerClientReportQuery request, CancellationToken cancellationToken) =>
        _repository.GetVisitsPerClientAsync(request.FromDate, request.ToDate, request.RequestingAuth0UserId, cancellationToken);
}
