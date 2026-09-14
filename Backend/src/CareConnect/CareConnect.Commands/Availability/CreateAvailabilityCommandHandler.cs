using AutoMapper;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Errors;
using CareConnect.Infrastructure.Repositories.Availability;
using MediatR;

namespace CareConnect.Commands.Availability;

public sealed class CreateAvailabilityCommandHandler : IRequestHandler<CreateAvailabilityCommand, int>
{
    private readonly ICaregiverAvailabilityRepository _repository;
    private readonly IMapper _mapper;

    public CreateAvailabilityCommandHandler(ICaregiverAvailabilityRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateAvailabilityCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Availability;

        var hasOverlap = await _repository.ExistsOverlappingWindowAsync(
            dto.CaregiverId, dto.DayOfWeek, dto.StartTime, dto.EndTime, excludingId: null, cancellationToken);

        if (hasOverlap)
        {
            throw new BusinessRuleViolationException(
                "This availability period overlaps with an existing one for that caregiver and day.");
        }

        var availability = _mapper.Map<CaregiverAvailability>(dto);

        _repository.Add(availability);
        await _repository.SaveChangesAsync(cancellationToken);

        return availability.Id;
    }
}
