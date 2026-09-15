using CareConnect.DTOs.Enums;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Errors;
using CareConnect.Infrastructure.Repositories.Visits;
using MediatR;

namespace CareConnect.Commands.Visits;

public sealed class UncompleteVisitTaskCommandHandler : IRequestHandler<UncompleteVisitTaskCommand>
{
    private readonly IVisitRepository _repository;

    public UncompleteVisitTaskCommandHandler(IVisitRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(UncompleteVisitTaskCommand request, CancellationToken cancellationToken)
    {
        var (visit, task) = await _repository.GetTaskAsync(request.VisitId, request.VisitTaskId, cancellationToken);

        var user = await _repository.GetUserByAuth0UserIdAsync(request.RequestingAuth0UserId, cancellationToken)
            ?? throw new ForbiddenException("This account is not recognized.");

        Caregiver? caregiver = user.Role == UserRole.Caregiver
            ? await _repository.GetCaregiverByAuth0UserIdAsync(request.RequestingAuth0UserId, cancellationToken)
            : null;

        VisitAccessControl.EnsureCanManageVisit(user, caregiver, visit);

        // The business rule gating this specific command: once a visit is closed out
        // (Completed/Cancelled/NoShow), its task record is history and can't be reopened.
        VisitTaskLookup.EnsureVisitIsEditable(visit);

        if (!task.IsCompleted)
        {
            throw new BusinessRuleException("This task is not marked complete.");
        }

        task.IsCompleted = false;
        task.CompletedAtUtc = null;
        task.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.SaveChangesAsync(cancellationToken);
    }
}
