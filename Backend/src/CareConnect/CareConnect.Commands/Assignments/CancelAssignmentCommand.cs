using MediatR;

namespace CareConnect.Commands.Assignments;

public sealed record CancelAssignmentCommand(int AssignmentId) : IRequest;
