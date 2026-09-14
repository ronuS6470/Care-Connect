using CareConnect.Infrastructure.Errors;
using CareConnect.Infrastructure.Repositories.Assignments;
using MediatR;

namespace CareConnect.Commands.Assignments;

public sealed class UpdateAssignmentCommandHandler : IRequestHandler<UpdateAssignmentCommand>
{
    private readonly IAssignmentRepository _repository;

    public UpdateAssignmentCommandHandler(IAssignmentRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(UpdateAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _repository.GetByIdAsync(request.AssignmentId, cancellationToken)
            ?? throw new NotFoundException($"Assignment {request.AssignmentId} was not found.");

        var dto = request.Assignment;

        // The DTO validator can't check this — it only carries StartDate for Create, and this
        // command's payload has no StartDate to compare against (only the stored assignment does).
        if (dto.EndDate.HasValue && dto.EndDate.Value < assignment.StartDate)
        {
            throw new BusinessRuleViolationException("EndDate must be on or after the assignment's StartDate.");
        }

        assignment.Status = dto.Status;
        assignment.EndDate = dto.EndDate;
        assignment.Notes = dto.Notes;
        assignment.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.SaveChangesAsync(cancellationToken);
    }
}
