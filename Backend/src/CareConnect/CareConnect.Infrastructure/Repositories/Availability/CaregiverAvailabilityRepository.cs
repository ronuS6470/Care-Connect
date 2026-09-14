using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Infrastructure.Repositories.Availability;

public sealed class CaregiverAvailabilityRepository : ICaregiverAvailabilityRepository
{
    private readonly CareConnectDbContext _dbContext;

    public CaregiverAvailabilityRepository(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<CaregiverAvailability?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        _dbContext.CaregiverAvailabilities.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public Task<bool> ExistsOverlappingWindowAsync(
        int caregiverId, DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? excludingId, CancellationToken cancellationToken) =>
        _dbContext.CaregiverAvailabilities.AnyAsync(a =>
            a.Id != excludingId &&
            a.CaregiverId == caregiverId &&
            a.DayOfWeek == dayOfWeek &&
            a.IsActive &&
            a.StartTime < endTime &&
            a.EndTime > startTime,
            cancellationToken);

    public void Add(CaregiverAvailability availability) => _dbContext.CaregiverAvailabilities.Add(availability);

    public void Remove(CaregiverAvailability availability) => _dbContext.CaregiverAvailabilities.Remove(availability);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
}
