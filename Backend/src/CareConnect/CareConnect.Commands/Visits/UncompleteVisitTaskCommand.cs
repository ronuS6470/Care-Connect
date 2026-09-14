using MediatR;

namespace CareConnect.Commands.Visits;

/// <summary>Reverses a completed task, "where business rules permit" — see VisitTaskLookup.EnsureVisitIsEditable.</summary>
public sealed record UncompleteVisitTaskCommand(int VisitId, int VisitTaskId, string RequestingAuth0UserId) : IRequest;
