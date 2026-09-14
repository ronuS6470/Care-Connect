using CareConnect.DTOs.Visits;
using MediatR;

namespace CareConnect.Commands.Visits;

public sealed record CompleteVisitCommand(int VisitId, string RequestingAuth0UserId, CompleteVisitDto Completion) : IRequest;
