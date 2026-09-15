using CareConnect.Infrastructure.Repositories.Visits;
using MediatR;

namespace CareConnect.Commands.Visits;

public sealed class CreateVisitCommandHandler : IRequestHandler<CreateVisitCommand, int>
{
    private readonly IVisitRepository _repository;

    public CreateVisitCommandHandler(IVisitRepository repository)
    {
        _repository = repository;
    }

    public Task<int> Handle(CreateVisitCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Visit;

        return _repository.CreateScheduledVisitAsync(
            dto.CaregiverAssignmentId,
            dto.ScheduledStartUtc,
            dto.ScheduledEndUtc,
            dto.CareTaskIds.Distinct().ToList(),
            cancellationToken);
    }
}
