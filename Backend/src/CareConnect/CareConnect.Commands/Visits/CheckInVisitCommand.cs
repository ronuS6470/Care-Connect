using MediatR;

namespace CareConnect.Commands.Visits;

/// <summary>
/// No client-supplied timestamp — CheckInTime is always the server's clock at the moment this
/// runs, never something the caregiver can dictate.
/// </summary>
public sealed record CheckInVisitCommand(int VisitId, string RequestingAuth0UserId) : IRequest;
