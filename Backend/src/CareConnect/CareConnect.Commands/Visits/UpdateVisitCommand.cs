using CareConnect.DTOs.Visits;
using MediatR;

namespace CareConnect.Commands.Visits;

/// <summary>Reschedules a Scheduled visit. Cannot change status — see CancelVisitCommand for that.</summary>
public sealed record UpdateVisitCommand(int VisitId, UpdateVisitDto Visit) : IRequest;
