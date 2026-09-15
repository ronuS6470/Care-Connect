using CareConnect.DTOs.Reporting;
using MediatR;

namespace CareConnect.Queries.Visits.GetTodaysVisits;

public sealed record GetTodaysVisitsQuery(string RequestingAuth0UserId) : IRequest<IReadOnlyList<VisitSummaryDto>>;
