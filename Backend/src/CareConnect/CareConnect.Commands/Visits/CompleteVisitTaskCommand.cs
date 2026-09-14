using MediatR;

namespace CareConnect.Commands.Visits;

public sealed record CompleteVisitTaskCommand(int VisitId, int VisitTaskId, string RequestingAuth0UserId) : IRequest;
