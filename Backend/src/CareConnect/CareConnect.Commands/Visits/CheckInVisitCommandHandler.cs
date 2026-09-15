using CareConnect.DTOs.Enums;
using CareConnect.Infrastructure.Errors;
using CareConnect.Infrastructure.Repositories.Visits;
using MediatR;

namespace CareConnect.Commands.Visits;

public sealed class CheckInVisitCommandHandler : IRequestHandler<CheckInVisitCommand>
{
    private readonly IVisitRepository _repository;

    public CheckInVisitCommandHandler(IVisitRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(CheckInVisitCommand request, CancellationToken cancellationToken)
    {
        var visit = await _repository.GetByIdAsync(request.VisitId, cancellationToken)
            ?? throw new NotFoundException($"Visit {request.VisitId} was not found.");

        var caregiver = await _repository.GetCaregiverByAuth0UserIdAsync(request.RequestingAuth0UserId, cancellationToken)
            ?? throw new ForbiddenException("This account is not recognized as a caregiver.");

        VisitOwnership.EnsureCallerOwnsVisit(caregiver, visit);

        if (visit.Status != VisitStatus.Scheduled)
        {
            throw new BusinessRuleException(
                $"Cannot check in: visit status is {visit.Status}, not Scheduled.");
        }

        var now = DateTime.UtcNow;
        var earliestAllowed = visit.ScheduledStartUtc.AddMinutes(-VisitExecutionPolicy.CheckInEarlyGraceMinutes);

        if (now < earliestAllowed)
        {
            throw new BusinessRuleException(
                $"Check-in is only allowed within {VisitExecutionPolicy.CheckInEarlyGraceMinutes} minutes of the scheduled start time.");
        }

        visit.ActualStartUtc = now;
        visit.Status = VisitStatus.InProgress;
        visit.UpdatedAtUtc = now;

        await _repository.SaveChangesAsync(cancellationToken);
    }
}
