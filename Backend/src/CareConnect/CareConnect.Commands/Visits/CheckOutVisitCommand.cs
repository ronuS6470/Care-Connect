using MediatR;

namespace CareConnect.Commands.Visits;

/// <summary>No client-supplied timestamp — CheckOutTime is always the server's clock.</summary>
public sealed record CheckOutVisitCommand(int VisitId, string RequestingAuth0UserId) : IRequest;
