using CareConnect.DTOs.Errors;
using CareConnect.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Commands.Caregivers;

public sealed class UpdateCaregiverCommandHandler : IRequestHandler<UpdateCaregiverCommand>
{
    private readonly CareConnectDbContext _dbContext;

    public UpdateCaregiverCommandHandler(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(UpdateCaregiverCommand request, CancellationToken cancellationToken)
    {
        var caregiver = await _dbContext.Caregivers
            .FirstOrDefaultAsync(c => c.Id == request.CaregiverId, cancellationToken)
            ?? throw new NotFoundException($"Caregiver {request.CaregiverId} was not found.");

        var dto = request.Caregiver;

        caregiver.LicenseNumber = dto.LicenseNumber;
        caregiver.HourlyRate = dto.HourlyRate;
        caregiver.HireDate = dto.HireDate;
        caregiver.YearsOfExperience = dto.YearsOfExperience;
        caregiver.IsActive = dto.IsActive;
        caregiver.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
