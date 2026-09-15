using AutoMapper;
using CareConnect.Infrastructure.Errors;
using CareConnect.Infrastructure.Repositories.CareTasks;
using MediatR;

namespace CareConnect.Commands.CareTasks;

public sealed class UpdateCareTaskCommandHandler : IRequestHandler<UpdateCareTaskCommand>
{
    private readonly ICareTaskRepository _repository;
    private readonly IMapper _mapper;

    public UpdateCareTaskCommandHandler(ICareTaskRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task Handle(UpdateCareTaskCommand request, CancellationToken cancellationToken)
    {
        var careTask = await _repository.GetByIdAsync(request.CareTaskId, cancellationToken)
            ?? throw new NotFoundException($"Care task {request.CareTaskId} was not found.");

        var dto = request.CareTask;

        var nameUsedByAnotherTask = await _repository.ExistsWithNameAsync(dto.Name, request.CareTaskId, cancellationToken);

        if (nameUsedByAnotherTask)
        {
            throw new ConflictException($"A care task named '{dto.Name}' already exists.");
        }

        _mapper.Map(dto, careTask);

        await _repository.SaveChangesAsync(cancellationToken);
    }
}
