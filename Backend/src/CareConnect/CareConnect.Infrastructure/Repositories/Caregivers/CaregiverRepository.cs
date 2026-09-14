using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Errors;
using CareConnect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Infrastructure.Repositories.Caregivers;

public sealed class CaregiverRepository : ICaregiverRepository
{
    private readonly CareConnectDbContext _dbContext;

    public CaregiverRepository(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Caregiver?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        _dbContext.Caregivers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public Task<User?> GetUserByIdAsync(int userId, CancellationToken cancellationToken) =>
        _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

    public Task<bool> IsEmailUsedByAnotherUserAsync(int userId, string email, CancellationToken cancellationToken) =>
        _dbContext.Users.AnyAsync(u => u.Id != userId && u.Email == email, cancellationToken);

    public Task<bool> ExistsForUserAsync(int userId, CancellationToken cancellationToken) =>
        _dbContext.Caregivers.AnyAsync(c => c.UserId == userId, cancellationToken);

    public void Add(Caregiver caregiver) => _dbContext.Caregivers.Add(caregiver);

    public async Task SetActiveStatusAsync(int caregiverId, bool isActive, CancellationToken cancellationToken)
    {
        var caregiver = await _dbContext.Caregivers
            .FirstOrDefaultAsync(c => c.Id == caregiverId, cancellationToken)
            ?? throw new NotFoundException($"Caregiver {caregiverId} was not found.");

        caregiver.IsActive = isActive;
        caregiver.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
}
