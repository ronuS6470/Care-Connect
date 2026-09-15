using CareConnect.DTOs.Enums;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Errors;
using CareConnect.Infrastructure.Repositories.Visits;
using MediatR;

namespace CareConnect.Commands.Visits;

public sealed class UpdateVisitTaskCommandHandler : IRequestHandler<UpdateVisitTaskCommand>
{
    private readonly IVisitRepository _repository;

    public UpdateVisitTaskCommandHandler(IVisitRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(UpdateVisitTaskCommand request, CancellationToken cancellationToken)
    {
        var (visit, task) = await _repository.GetTaskAsync(request.VisitId, request.VisitTaskId, cancellationToken);

        var user = await _repository.GetUserByAuth0UserIdAsync(request.RequestingAuth0UserId, cancellationToken)
            ?? throw new ForbiddenException("This account is not recognized.");

        Caregiver? caregiver = user.Role == UserRole.Caregiver
            ? await _repository.GetCaregiverByAuth0UserIdAsync(request.RequestingAuth0UserId, cancellationToken)
            : null;

        VisitAccessControl.EnsureCanManageVisit(user, caregiver, visit);
        VisitTaskLookup.EnsureVisitIsEditable(visit);

        task.Notes = request.Task.Notes;
        task.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.SaveChangesAsync(cancellationToken);
    }
}
