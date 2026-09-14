using CareConnect.DTOs.Assignments;
using MediatR;

namespace CareConnect.Commands.Assignments;

public sealed record CreateAssignmentCommand(CreateAssignmentDto Assignment) : IRequest<int>;
