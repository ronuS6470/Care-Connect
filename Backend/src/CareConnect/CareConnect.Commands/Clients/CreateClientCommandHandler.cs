using CareConnect.DTOs.Enums;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Errors;
using CareConnect.Infrastructure.Repositories.Clients;
using MediatR;

namespace CareConnect.Commands.Clients;

public sealed class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, int>
{
    private readonly IClientRepository _repository;

    public CreateClientCommandHandler(IClientRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Client;

        var user = await _repository.GetUserByIdAsync(dto.UserId, cancellationToken)
            ?? throw new NotFoundException($"User {dto.UserId} was not found.");

        if (user.Role != UserRole.Client)
        {
            throw new BusinessRuleViolationException("The linked user's role must be Client.");
        }

        var emailAlreadyUsedByAnotherUser = await _repository.IsEmailUsedByAnotherUserAsync(user.Id, user.Email, cancellationToken);

        if (emailAlreadyUsedByAnotherUser)
        {
            throw new BusinessRuleViolationException("Email must be unique.");
        }

        var alreadyHasClientProfile = await _repository.ExistsForUserAsync(dto.UserId, cancellationToken);

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

        _repository.Add(client);
        await _repository.SaveChangesAsync(cancellationToken);

        return client.Id;
    }
}
