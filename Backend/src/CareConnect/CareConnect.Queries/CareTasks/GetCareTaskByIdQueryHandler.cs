using CareConnect.DTOs.CareTasks;
using CareConnect.Queries.CareTasks.Repositories;
using MediatR;

namespace CareConnect.Queries.CareTasks;

public sealed class GetCareTaskByIdQueryHandler : IRequestHandler<GetCareTaskByIdQuery, CareTaskDto?>
{
    private readonly ICareTaskReadRepository _repository;

    public GetCareTaskByIdQueryHandler(ICareTaskReadRepository repository)
    {
        _repository = repository;
    }

    public Task<CareTaskDto?> Handle(GetCareTaskByIdQuery request, CancellationToken cancellationToken) =>
        _repository.GetByIdAsync(request.CareTaskId, cancellationToken);
}
