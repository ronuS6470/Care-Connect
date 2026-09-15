using CareConnect.DTOs.Visits;
using MediatR;

namespace CareConnect.Queries.Visits.GetVisitById;

public sealed record GetVisitByIdQuery(int VisitId, string RequestingAuth0UserId) : IRequest<VisitDto?>;
