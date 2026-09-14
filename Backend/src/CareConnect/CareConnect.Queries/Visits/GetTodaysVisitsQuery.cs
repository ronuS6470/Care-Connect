using CareConnect.DTOs.Reporting;
using MediatR;

namespace CareConnect.Queries.Visits;

public sealed record GetTodaysVisitsQuery(string RequestingAuth0UserId) : IRequest<IReadOnlyList<VisitSummaryDto>>;
