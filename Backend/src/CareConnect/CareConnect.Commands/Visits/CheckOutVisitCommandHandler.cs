using CareConnect.DTOs.Enums;
using CareConnect.Infrastructure.Errors;
using CareConnect.Infrastructure.Repositories.Visits;
using MediatR;

namespace CareConnect.Commands.Visits;

public sealed class CheckOutVisitCommandHandler : IRequestHandler<CheckOutVisitCommand>
{
    private readonly IVisitRepository _repository;

    public CheckOutVisitCommandHandler(IVisitRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(CheckOutVisitCommand request, CancellationToken cancellationToken)
    {
        var visit = await _repository.GetByIdAsync(request.VisitId, cancellationToken)
            ?? throw new NotFoundException($"Visit {request.VisitId} was not found.");

        var caregiver = await _repository.GetCaregiverByAuth0UserIdAsync(request.RequestingAuth0UserId, cancellationToken)
            ?? throw new ForbiddenException("This account is not recognized as a caregiver.");

        VisitOwnership.EnsureCallerOwnsVisit(caregiver, visit);

        if (visit.Status != VisitStatus.InProgress)
        {
            throw new BusinessRuleViolationException(
                $"Cannot check out: visit status is {visit.Status}, not InProgress.");
        }

        if (visit.ActualStartUtc is null)
        {
            throw new BusinessRuleViolationException("This visit has no recorded check-in time.");
        }

        if (visit.ActualEndUtc is not null)
        {
            throw new BusinessRuleViolationException("This visit has already been checked out.");
        }

        var now = DateTime.UtcNow;

        if (now <= visit.ActualStartUtc.Value)
        {
            throw new BusinessRuleViolationException("Check-out time must be after check-in time.");
        }

        visit.ActualEndUtc = now;
        visit.UpdatedAtUtc = now;

        await _repository.SaveChangesAsync(cancellationToken);
    }
}
