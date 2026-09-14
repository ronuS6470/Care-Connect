using CareConnect.DTOs.Visits;
using MediatR;

namespace CareConnect.Queries.Visits;

public sealed record GetVisitNotesQuery(int VisitId, string RequestingAuth0UserId) : IRequest<IReadOnlyList<VisitNoteDto>>;
