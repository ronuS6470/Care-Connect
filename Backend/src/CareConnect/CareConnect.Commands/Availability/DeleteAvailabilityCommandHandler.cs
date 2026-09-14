using CareConnect.DTOs.Errors;
using CareConnect.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Commands.Availability;

public sealed class DeleteAvailabilityCommandHandler : IRequestHandler<DeleteAvailabilityCommand>
{
    private readonly CareConnectDbContext _dbContext;

    public DeleteAvailabilityCommandHandler(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(DeleteAvailabilityCommand request, CancellationToken cancellationToken)
    {
        var availability = await _dbContext.CaregiverAvailabilities
            .FirstOrDefaultAsync(a => a.Id == request.AvailabilityId, cancellationToken)
            ?? throw new NotFoundException($"Availability {request.AvailabilityId} was not found.");

        _dbContext.CaregiverAvailabilities.Remove(availability);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
