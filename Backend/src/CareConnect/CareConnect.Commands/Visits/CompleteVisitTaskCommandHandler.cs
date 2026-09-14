using CareConnect.DTOs.Enums;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Errors;
using CareConnect.Infrastructure.Repositories.Visits;
using MediatR;

namespace CareConnect.Commands.Visits;

public sealed class CompleteVisitTaskCommandHandler : IRequestHandler<CompleteVisitTaskCommand>
{
    private readonly IVisitRepository _repository;

    public CompleteVisitTaskCommandHandler(IVisitRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(CompleteVisitTaskCommand request, CancellationToken cancellationToken)
    {
        var (visit, task) = await _repository.GetTaskAsync(request.VisitId, request.VisitTaskId, cancellationToken);

        var user = await _repository.GetUserByAuth0UserIdAsync(request.RequestingAuth0UserId, cancellationToken)
            ?? throw new ForbiddenException("This account is not recognized.");

        Caregiver? caregiver = user.Role == UserRole.Caregiver
            ? await _repository.GetCaregiverByAuth0UserIdAsync(request.RequestingAuth0UserId, cancellationToken)
            : null;

        VisitAccessControl.EnsureCanManageVisit(user, caregiver, visit);
        VisitTaskLookup.EnsureVisitIsEditable(visit);

        if (task.IsCompleted)
        {
            throw new BusinessRuleViolationException("This task is already marked complete.");
        }

        var now = DateTime.UtcNow;
        task.IsCompleted = true;
        task.CompletedAtUtc = now;
        task.UpdatedAtUtc = now;

        await _repository.SaveChangesAsync(cancellationToken);
    }
}
