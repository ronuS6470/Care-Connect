using CareConnect.DTOs.Visits;
using MediatR;

namespace CareConnect.Queries.Visits.GetVisitTasks;

public sealed record GetVisitTasksQuery(int VisitId, string RequestingAuth0UserId) : IRequest<IReadOnlyList<VisitTaskDto>>;
