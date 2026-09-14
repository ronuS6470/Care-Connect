using CareConnect.DTOs.Enums;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Errors;
using CareConnect.Infrastructure.Repositories.Assignments;
using CareConnect.Infrastructure.Repositories.Caregivers;
using CareConnect.Infrastructure.Repositories.Clients;
using MediatR;

namespace CareConnect.Commands.Assignments;

public sealed class CreateAssignmentCommandHandler : IRequestHandler<CreateAssignmentCommand, int>
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly ICaregiverRepository _caregiverRepository;
    private readonly IClientRepository _clientRepository;

    public CreateAssignmentCommandHandler(
        IAssignmentRepository assignmentRepository,
        ICaregiverRepository caregiverRepository,
        IClientRepository clientRepository)
    {
        _assignmentRepository = assignmentRepository;
        _caregiverRepository = caregiverRepository;
        _clientRepository = clientRepository;
    }

    public async Task<int> Handle(CreateAssignmentCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Assignment;

        var caregiver = await _caregiverRepository.GetByIdAsync(dto.CaregiverId, cancellationToken)
            ?? throw new NotFoundException($"Caregiver {dto.CaregiverId} was not found.");

        var client = await _clientRepository.GetByIdAsync(dto.ClientId, cancellationToken)
            ?? throw new NotFoundException($"Client {dto.ClientId} was not found.");

        if (!caregiver.IsActive)
        {
            throw new BusinessRuleViolationException("Caregiver is not active.");
        }

        if (!client.IsActive)
        {
            throw new BusinessRuleViolationException("Client is not active.");
        }

        var hasActiveAssignment = await _assignmentRepository.ExistsActiveAssignmentAsync(
            dto.CaregiverId, dto.ClientId, cancellationToken);

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

        _assignmentRepository.Add(assignment);
        await _assignmentRepository.SaveChangesAsync(cancellationToken);

        return assignment.Id;
    }
}
