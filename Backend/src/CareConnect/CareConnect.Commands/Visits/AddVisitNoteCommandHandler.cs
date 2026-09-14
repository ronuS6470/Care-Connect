using CareConnect.DTOs.Errors;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Commands.Visits;

public sealed class AddVisitNoteCommandHandler : IRequestHandler<AddVisitNoteCommand, int>
{
    private readonly CareConnectDbContext _dbContext;

    public AddVisitNoteCommandHandler(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> Handle(AddVisitNoteCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Note;

        var visit = await _dbContext.Visits
            .Include(v => v.CaregiverAssignment)
            .FirstOrDefaultAsync(v => v.Id == dto.VisitId, cancellationToken)
            ?? throw new NotFoundException($"Visit {dto.VisitId} was not found.");

        var author = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Auth0UserId == request.RequestingAuth0UserId, cancellationToken)
            ?? throw new ForbiddenException("This account is not recognized.");

        // Same rule as managing tasks: Admin or the assigned caregiver only — a Client is
        // read-only for notes, they don't author them.
        await VisitAccessControl.EnsureCanManageVisitAsync(_dbContext, visit, request.RequestingAuth0UserId, cancellationToken);

        var note = new VisitNote
        {
            VisitId = dto.VisitId,
            AuthorUserId = author.Id,
            Content = dto.Content,
        };

        _dbContext.VisitNotes.Add(note);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return note.Id;
    }
}
