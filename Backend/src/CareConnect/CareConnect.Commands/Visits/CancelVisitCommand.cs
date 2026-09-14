using CareConnect.DTOs.Visits;
using MediatR;

namespace CareConnect.Commands.Visits;

public sealed record CancelVisitCommand(int VisitId, CancelVisitDto Cancellation) : IRequest;
