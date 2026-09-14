using CareConnect.DTOs.Errors;
using CareConnect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.AppServices.Caregivers;

public sealed class CaregiverActivationService : ICaregiverActivationService
{
    private readonly CareConnectDbContext _dbContext;

    public CaregiverActivationService(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SetActiveStatusAsync(int caregiverId, bool isActive, CancellationToken cancellationToken)
    {
        var caregiver = await _dbContext.Caregivers
            .FirstOrDefaultAsync(c => c.Id == caregiverId, cancellationToken)
            ?? throw new NotFoundException($"Caregiver {caregiverId} was not found.");

        caregiver.IsActive = isActive;
        caregiver.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
