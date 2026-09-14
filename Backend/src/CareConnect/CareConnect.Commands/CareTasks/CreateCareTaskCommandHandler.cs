using AutoMapper;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Errors;
using CareConnect.Infrastructure.Repositories.CareTasks;
using MediatR;

namespace CareConnect.Commands.CareTasks;

public sealed class CreateCareTaskCommandHandler : IRequestHandler<CreateCareTaskCommand, int>
{
    private readonly ICareTaskRepository _repository;
    private readonly IMapper _mapper;

    public CreateCareTaskCommandHandler(ICareTaskRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateCareTaskCommand request, CancellationToken cancellationToken)
    {
        var dto = request.CareTask;

        var nameAlreadyExists = await _repository.ExistsWithNameAsync(dto.Name, excludingId: null, cancellationToken);

        if (nameAlreadyExists)
        {
            throw new BusinessRuleViolationException($"A care task named '{dto.Name}' already exists.");
        }

        var careTask = _mapper.Map<CareTask>(dto);

        _repository.Add(careTask);
        await _repository.SaveChangesAsync(cancellationToken);

        return careTask.Id;
    }
}
