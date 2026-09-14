using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Errors;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Commands.Assignments;

public sealed class CreateAssignmentCommandHandler : IRequestHandler<CreateAssignmentCommand, int>
{
    private readonly CareConnectDbContext _dbContext;

    public CreateAssignmentCommandHandler(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> Handle(CreateAssignmentCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Assignment;

        var caregiver = await _dbContext.Caregivers.FirstOrDefaultAsync(c => c.Id == dto.CaregiverId, cancellationToken)
            ?? throw new NotFoundException($"Caregiver {dto.CaregiverId} was not found.");

        var client = await _dbContext.Clients.FirstOrDefaultAsync(c => c.Id == dto.ClientId, cancellationToken)
            ?? throw new NotFoundException($"Client {dto.ClientId} was not found.");

        if (!caregiver.IsActive)
        {
            throw new BusinessRuleViolationException("Caregiver is not active.");
        }

        if (!client.IsActive)
        {
            throw new BusinessRuleViolationException("Client is not active.");
        }

        var hasActiveAssignment = await _dbContext.CaregiverAssignments.AnyAsync(a =>
            a.CaregiverId == dto.CaregiverId &&
            a.ClientId == dto.ClientId &&
            a.Status == AssignmentStatus.Active,
            cancellationToken);

        if (hasActiveAssignment)
        {
            throw new BusinessRuleViolationException("This caregiver already has an active assignment with this client.");
        }

        var assignment = new CaregiverAssignment
        {
            CaregiverId = dto.CaregiverId,
            ClientId = dto.ClientId,
            Status = AssignmentStatus.Active,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Notes = dto.Notes,
        };

        _dbContext.CaregiverAssignments.Add(assignment);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return assignment.Id;
    }
}
