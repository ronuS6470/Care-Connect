using CareConnect.DTOs.Errors;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Commands.Availability;

public sealed class CreateAvailabilityCommandHandler : IRequestHandler<CreateAvailabilityCommand, int>
{
    private readonly CareConnectDbContext _dbContext;

    public CreateAvailabilityCommandHandler(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> Handle(CreateAvailabilityCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Availability;

        var hasOverlap = await _dbContext.CaregiverAvailabilities.AnyAsync(a =>
            a.CaregiverId == dto.CaregiverId &&
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

        var availability = new CaregiverAvailability
        {
            CaregiverId = dto.CaregiverId,
            DayOfWeek = dto.DayOfWeek,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            IsActive = true,
        };

        _dbContext.CaregiverAvailabilities.Add(availability);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return availability.Id;
    }
}
