using CareConnect.DTOs.Errors;
using CareConnect.DTOs.Enums;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Commands.Caregivers;

public sealed class CreateCaregiverCommandHandler : IRequestHandler<CreateCaregiverCommand, int>
{
    private readonly CareConnectDbContext _dbContext;

    public CreateCaregiverCommandHandler(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> Handle(CreateCaregiverCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Caregiver;

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == dto.UserId, cancellationToken)
            ?? throw new NotFoundException($"User {dto.UserId} was not found.");

        if (user.Role != UserRole.Caregiver)
        {
            throw new BusinessRuleViolationException("The linked user's role must be Caregiver.");
        }

        var emailAlreadyUsedByAnotherUser = await _dbContext.Users
            .AnyAsync(u => u.Id != user.Id && u.Email == user.Email, cancellationToken);

        if (emailAlreadyUsedByAnotherUser)
        {
            throw new BusinessRuleViolationException("Email must be unique.");
        }

        var alreadyHasCaregiverProfile = await _dbContext.Caregivers
            .AnyAsync(c => c.UserId == dto.UserId, cancellationToken);

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

        _dbContext.Caregivers.Add(caregiver);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return caregiver.Id;
    }
}
