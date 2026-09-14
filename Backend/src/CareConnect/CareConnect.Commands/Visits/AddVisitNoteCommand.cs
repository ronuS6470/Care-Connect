using CareConnect.DTOs.Visits;
using MediatR;

namespace CareConnect.Commands.Visits;

/// <summary>Returns the new note's Id. The author is always the authenticated caller, never a client-supplied field.</summary>
public sealed record AddVisitNoteCommand(CreateVisitNoteDto Note, string RequestingAuth0UserId) : IRequest<int>;
