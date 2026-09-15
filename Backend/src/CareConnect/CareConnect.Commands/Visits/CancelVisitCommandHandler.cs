using CareConnect.DTOs.Enums;
using CareConnect.Infrastructure.Errors;
using CareConnect.Infrastructure.Repositories.Visits;
using MediatR;

namespace CareConnect.Commands.Visits;

public sealed class CancelVisitCommandHandler : IRequestHandler<CancelVisitCommand>
{
    private readonly IVisitRepository _repository;

    public CancelVisitCommandHandler(IVisitRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(CancelVisitCommand request, CancellationToken cancellationToken)
    {
        var visit = await _repository.GetByIdAsync(request.VisitId, cancellationToken)
            ?? throw new NotFoundException($"Visit {request.VisitId} was not found.");

        if (visit.Status is VisitStatus.Cancelled or VisitStatus.Completed)
        {
            throw new BusinessRuleException($"A visit with status {visit.Status} cannot be cancelled.");
        }

        visit.Status = VisitStatus.Cancelled;
        visit.CancellationReason = request.Cancellation.CancellationReason;
        visit.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.SaveChangesAsync(cancellationToken);
    }
}
