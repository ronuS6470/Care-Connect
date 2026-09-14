using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Errors;
using CareConnect.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Commands.Assignments;

public sealed class CancelAssignmentCommandHandler : IRequestHandler<CancelAssignmentCommand>
{
    private readonly CareConnectDbContext _dbContext;

    public CancelAssignmentCommandHandler(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(CancelAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _dbContext.CaregiverAssignments
            .FirstOrDefaultAsync(a => a.Id == request.AssignmentId, cancellationToken)
            ?? throw new NotFoundException($"Assignment {request.AssignmentId} was not found.");

        if (assignment.Status == AssignmentStatus.Cancelled)
        {
            throw new BusinessRuleViolationException("This assignment is already cancelled.");
        }

        assignment.Status = AssignmentStatus.Cancelled;
        assignment.EndDate ??= DateOnly.FromDateTime(DateTime.UtcNow);
        assignment.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
