using AutoMapper;
using CareConnect.Infrastructure.Errors;
using CareConnect.Infrastructure.Repositories.Caregivers;
using MediatR;

namespace CareConnect.Commands.Caregivers;

public sealed class UpdateCaregiverCommandHandler : IRequestHandler<UpdateCaregiverCommand>
{
    private readonly ICaregiverRepository _repository;
    private readonly IMapper _mapper;

    public UpdateCaregiverCommandHandler(ICaregiverRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task Handle(UpdateCaregiverCommand request, CancellationToken cancellationToken)
    {
        var caregiver = await _repository.GetByIdAsync(request.CaregiverId, cancellationToken)
            ?? throw new NotFoundException($"Caregiver {request.CaregiverId} was not found.");

        _mapper.Map(request.Caregiver, caregiver);

        await _repository.SaveChangesAsync(cancellationToken);
    }
}
