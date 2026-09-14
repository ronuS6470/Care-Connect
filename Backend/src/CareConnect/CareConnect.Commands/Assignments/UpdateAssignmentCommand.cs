using CareConnect.DTOs.Assignments;
using MediatR;

namespace CareConnect.Commands.Assignments;

public sealed record UpdateAssignmentCommand(int AssignmentId, UpdateAssignmentDto Assignment) : IRequest;
