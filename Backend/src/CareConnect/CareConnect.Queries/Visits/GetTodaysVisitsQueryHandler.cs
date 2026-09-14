using CareConnect.DTOs.Reporting;
using MediatR;

namespace CareConnect.Queries.Visits;

/// <summary>
/// "Today" is just GetVisitsQuery with FromDate=ToDate=today — delegating keeps the SQL,
/// pagination, and authorization scoping in exactly one place.
/// </summary>
public sealed class GetTodaysVisitsQueryHandler : IRequestHandler<GetTodaysVisitsQuery, IReadOnlyList<VisitSummaryDto>>
{
    private readonly IMediator _mediator;

    public GetTodaysVisitsQueryHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IReadOnlyList<VisitSummaryDto>> Handle(GetTodaysVisitsQuery request, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var page = await _mediator.Send(
            new GetVisitsQuery(1, 200, today, today, null, null, null, null, request.RequestingAuth0UserId),
            cancellationToken);

        return page.Data;
    }
}
