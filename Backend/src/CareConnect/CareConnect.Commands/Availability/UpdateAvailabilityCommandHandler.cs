using AutoMapper;
using CareConnect.Infrastructure.Errors;
using CareConnect.Infrastructure.Repositories.Availability;
using MediatR;

namespace CareConnect.Commands.Availability;

public sealed class UpdateAvailabilityCommandHandler : IRequestHandler<UpdateAvailabilityCommand>
{
    private readonly ICaregiverAvailabilityRepository _repository;
    private readonly IMapper _mapper;

    public UpdateAvailabilityCommandHandler(ICaregiverAvailabilityRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task Handle(UpdateAvailabilityCommand request, CancellationToken cancellationToken)
    {
        var availability = await _repository.GetByIdAsync(request.AvailabilityId, cancellationToken)
            ?? throw new NotFoundException($"Availability {request.AvailabilityId} was not found.");

        var dto = request.Availability;

        var hasOverlap = await _repository.ExistsOverlappingWindowAsync(
            availability.CaregiverId, dto.DayOfWeek, dto.StartTime, dto.EndTime, request.AvailabilityId, cancellationToken);

        if (hasOverlap)
        {
            throw new BusinessRuleViolationException(
                "This availability period overlaps with an existing one for that caregiver and day.");
        }

        _mapper.Map(dto, availability);

        await _repository.SaveChangesAsync(cancellationToken);
    }
}
