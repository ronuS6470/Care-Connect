using CareConnect.DTOs.Visits;
using MediatR;

namespace CareConnect.Queries.Visits;

public sealed record GetVisitByIdQuery(int VisitId, string RequestingAuth0UserId) : IRequest<VisitDto?>;
