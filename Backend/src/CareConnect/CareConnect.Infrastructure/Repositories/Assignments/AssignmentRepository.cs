using CareConnect.DTOs.Enums;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Infrastructure.Repositories.Assignments;

public sealed class AssignmentRepository : IAssignmentRepository
{
    private readonly CareConnectDbContext _dbContext;

    public AssignmentRepository(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<CaregiverAssignment?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        _dbContext.CaregiverAssignments.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public Task<bool> ExistsActiveAssignmentAsync(int caregiverId, int clientId, CancellationToken cancellationToken) =>
        _dbContext.CaregiverAssignments.AnyAsync(a =>
            a.CaregiverId == caregiverId &&
            a.ClientId == clientId &&
            a.Status == AssignmentStatus.Active,
            cancellationToken);

    public void Add(CaregiverAssignment assignment) => _dbContext.CaregiverAssignments.Add(assignment);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
}
