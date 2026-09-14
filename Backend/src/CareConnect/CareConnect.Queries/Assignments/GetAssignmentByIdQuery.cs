using CareConnect.DTOs.Assignments;
using MediatR;

namespace CareConnect.Queries.Assignments;

public sealed record GetAssignmentByIdQuery(int AssignmentId, string RequestingAuth0UserId) : IRequest<AssignmentDto?>;
