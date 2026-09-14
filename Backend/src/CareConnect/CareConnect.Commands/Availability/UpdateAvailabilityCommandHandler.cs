using CareConnect.DTOs.Errors;
using CareConnect.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Commands.Availability;

public sealed class UpdateAvailabilityCommandHandler : IRequestHandler<UpdateAvailabilityCommand>
{
    private readonly CareConnectDbContext _dbContext;

    public UpdateAvailabilityCommandHandler(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(UpdateAvailabilityCommand request, CancellationToken cancellationToken)
    {
        var availability = await _dbContext.CaregiverAvailabilities
            .FirstOrDefaultAsync(a => a.Id == request.AvailabilityId, cancellationToken)
            ?? throw new NotFoundException($"Availability {request.AvailabilityId} was not found.");

        var dto = request.Availability;

        var hasOverlap = await _dbContext.CaregiverAvailabilities.AnyAsync(a =>
            a.Id != request.AvailabilityId &&
            a.CaregiverId == availability.CaregiverId &&
            a.DayOfWeek == dto.DayOfWeek &&
            a.IsActive &&
            a.StartTime < dto.EndTime &&
            a.EndTime > dto.StartTime,
            cancellationToken);

        if (hasOverlap)
        {
            throw new BusinessRuleViolationException(
                "This availability period overlaps with an existing one for that caregiver and day.");
        }

        availability.DayOfWeek = dto.DayOfWeek;
        availability.StartTime = dto.StartTime;
        availability.EndTime = dto.EndTime;
        availability.IsActive = dto.IsActive;
        availability.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
