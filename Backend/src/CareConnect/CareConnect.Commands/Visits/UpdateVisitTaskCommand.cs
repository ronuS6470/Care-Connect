using CareConnect.DTOs.Visits;
using MediatR;

namespace CareConnect.Commands.Visits;

public sealed record UpdateVisitTaskCommand(int VisitId, int VisitTaskId, UpdateVisitTaskDto Task, string RequestingAuth0UserId) : IRequest;
