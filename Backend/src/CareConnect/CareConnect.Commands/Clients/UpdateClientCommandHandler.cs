using CareConnect.DTOs.Errors;
using CareConnect.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Commands.Clients;

public sealed class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand>
{
    private readonly CareConnectDbContext _dbContext;

    public UpdateClientCommandHandler(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        var client = await _dbContext.Clients
            .FirstOrDefaultAsync(c => c.Id == request.ClientId, cancellationToken)
            ?? throw new NotFoundException($"Client {request.ClientId} was not found.");

        var dto = request.Client;

        client.AddressLine1 = dto.AddressLine1;
        client.AddressLine2 = dto.AddressLine2;
        client.City = dto.City;
        client.State = dto.State;
        client.PostalCode = dto.PostalCode;
        client.EmergencyContactName = dto.EmergencyContactName;
        client.EmergencyContactPhone = dto.EmergencyContactPhone;
        client.IsActive = dto.IsActive;
        client.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
