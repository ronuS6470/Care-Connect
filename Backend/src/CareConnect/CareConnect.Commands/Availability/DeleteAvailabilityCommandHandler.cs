using CareConnect.Infrastructure.Errors;
using CareConnect.Infrastructure.Repositories.Availability;
using MediatR;

namespace CareConnect.Commands.Availability;

public sealed class DeleteAvailabilityCommandHandler : IRequestHandler<DeleteAvailabilityCommand>
{
    private readonly ICaregiverAvailabilityRepository _repository;

    public DeleteAvailabilityCommandHandler(ICaregiverAvailabilityRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeleteAvailabilityCommand request, CancellationToken cancellationToken)
    {
        var availability = await _repository.GetByIdAsync(request.AvailabilityId, cancellationToken)
            ?? throw new NotFoundException($"Availability {request.AvailabilityId} was not found.");

        _repository.Remove(availability);
        await _repository.SaveChangesAsync(cancellationToken);
    }
}
