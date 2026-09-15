using CareConnect.AppServices.Security;
using CareConnect.Commands.Visits;
using CareConnect.DTOs.Visits;
using CareConnect.Queries.Visits.GetVisitNotes;
using MediatR;

namespace CareConnect.AppServices.Visits;

public sealed class VisitNotesAppService : IVisitNotesAppService
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public VisitNotesAppService(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    {
        _mediator = mediator;
        _currentUserAccessor = currentUserAccessor;
    }

    public Task<IReadOnlyList<VisitNoteDto>> GetVisitNotesAsync(int visitId, CancellationToken cancellationToken) =>
        _mediator.Send(new GetVisitNotesQuery(visitId, _currentUserAccessor.Auth0UserId), cancellationToken);

    public Task<int> AddVisitNoteAsync(CreateVisitNoteDto dto, CancellationToken cancellationToken) =>
        _mediator.Send(new AddVisitNoteCommand(dto, _currentUserAccessor.Auth0UserId), cancellationToken);
}
