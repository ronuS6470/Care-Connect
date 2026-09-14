using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Errors;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Commands.Clients;

public sealed class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, int>
{
    private readonly CareConnectDbContext _dbContext;

    public CreateClientCommandHandler(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Client;

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == dto.UserId, cancellationToken)
            ?? throw new NotFoundException($"User {dto.UserId} was not found.");

        if (user.Role != UserRole.Client)
        {
            throw new BusinessRuleViolationException("The linked user's role must be Client.");
        }

        var emailAlreadyUsedByAnotherUser = await _dbContext.Users
            .AnyAsync(u => u.Id != user.Id && u.Email == user.Email, cancellationToken);

        if (emailAlreadyUsedByAnotherUser)
        {
            throw new BusinessRuleViolationException("Email must be unique.");
        }

        var alreadyHasClientProfile = await _dbContext.Clients
            .AnyAsync(c => c.UserId == dto.UserId, cancellationToken);

        if (alreadyHasClientProfile)
        {
            throw new BusinessRuleViolationException($"User {dto.UserId} already has a client profile.");
        }

        var client = new Client
        {
            UserId = dto.UserId,
            AddressLine1 = dto.AddressLine1,
            AddressLine2 = dto.AddressLine2,
            City = dto.City,
            State = dto.State,
            PostalCode = dto.PostalCode,
            EmergencyContactName = dto.EmergencyContactName,
            EmergencyContactPhone = dto.EmergencyContactPhone,
            IsActive = true,
        };

        _dbContext.Clients.Add(client);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return client.Id;
    }
}
