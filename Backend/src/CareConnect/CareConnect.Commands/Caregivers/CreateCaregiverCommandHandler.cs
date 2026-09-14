using CareConnect.DTOs.Enums;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Errors;
using CareConnect.Infrastructure.Repositories.Caregivers;
using MediatR;

namespace CareConnect.Commands.Caregivers;

public sealed class CreateCaregiverCommandHandler : IRequestHandler<CreateCaregiverCommand, int>
{
    private readonly ICaregiverRepository _repository;

    public CreateCaregiverCommandHandler(ICaregiverRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(CreateCaregiverCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Caregiver;

        var user = await _repository.GetUserByIdAsync(dto.UserId, cancellationToken)
            ?? throw new NotFoundException($"User {dto.UserId} was not found.");

        if (user.Role != UserRole.Caregiver)
        {
            throw new BusinessRuleViolationException("The linked user's role must be Caregiver.");
        }

        var emailAlreadyUsedByAnotherUser = await _repository.IsEmailUsedByAnotherUserAsync(user.Id, user.Email, cancellationToken);

        if (emailAlreadyUsedByAnotherUser)
        {
            throw new BusinessRuleViolationException("Email must be unique.");
        }

        var alreadyHasCaregiverProfile = await _repository.ExistsForUserAsync(dto.UserId, cancellationToken);

        if (alreadyHasCaregiverProfile)
        {
            throw new BusinessRuleViolationException($"User {dto.UserId} already has a caregiver profile.");
        }

        var caregiver = new Caregiver
        {
            UserId = dto.UserId,
            LicenseNumber = dto.LicenseNumber,
            HourlyRate = dto.HourlyRate,
            HireDate = dto.HireDate,
            DateOfBirth = dto.DateOfBirth,
            YearsOfExperience = dto.YearsOfExperience,
            IsActive = true,
        };

        _repository.Add(caregiver);
        await _repository.SaveChangesAsync(cancellationToken);

        return caregiver.Id;
    }
}
