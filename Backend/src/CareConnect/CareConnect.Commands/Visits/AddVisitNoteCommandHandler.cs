using CareConnect.DTOs.Enums;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Errors;
using CareConnect.Infrastructure.Repositories.Visits;
using MediatR;

namespace CareConnect.Commands.Visits;

public sealed class AddVisitNoteCommandHandler : IRequestHandler<AddVisitNoteCommand, int>
{
    private readonly IVisitRepository _repository;

    public AddVisitNoteCommandHandler(IVisitRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(AddVisitNoteCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Note;

        var visit = await _repository.GetByIdAsync(dto.VisitId, cancellationToken)
            ?? throw new NotFoundException($"Visit {dto.VisitId} was not found.");

        var author = await _repository.GetUserByAuth0UserIdAsync(request.RequestingAuth0UserId, cancellationToken)
            ?? throw new ForbiddenException("This account is not recognized.");

        // Same rule as managing tasks: Admin or the assigned caregiver only — a Client is
        // read-only for notes, they don't author them.
        Caregiver? caregiver = author.Role == UserRole.Caregiver
            ? await _repository.GetCaregiverByAuth0UserIdAsync(request.RequestingAuth0UserId, cancellationToken)
            : null;

        VisitAccessControl.EnsureCanManageVisit(author, caregiver, visit);

        var note = new VisitNote
        {
            VisitId = dto.VisitId,
            AuthorUserId = author.Id,
            Content = dto.Content,
        };

        _repository.AddVisitNote(note);
        await _repository.SaveChangesAsync(cancellationToken);

        return note.Id;
    }
}
