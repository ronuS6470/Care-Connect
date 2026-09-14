using CareConnect.DTOs.Enums;
using CareConnect.Infrastructure.Errors;
using CareConnect.Infrastructure.Repositories.Assignments;
using MediatR;

namespace CareConnect.Commands.Assignments;

public sealed class CancelAssignmentCommandHandler : IRequestHandler<CancelAssignmentCommand>
{
    private readonly IAssignmentRepository _repository;

    public CancelAssignmentCommandHandler(IAssignmentRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(CancelAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _repository.GetByIdAsync(request.AssignmentId, cancellationToken)
            ?? throw new NotFoundException($"Assignment {request.AssignmentId} was not found.");

        if (assignment.Status == AssignmentStatus.Cancelled)
        {
            throw new BusinessRuleViolationException("This assignment is already cancelled.");
        }

        assignment.Status = AssignmentStatus.Cancelled;
        assignment.EndDate ??= DateOnly.FromDateTime(DateTime.UtcNow);
        assignment.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.SaveChangesAsync(cancellationToken);
    }
}
