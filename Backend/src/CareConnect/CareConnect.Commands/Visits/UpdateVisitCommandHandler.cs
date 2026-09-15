using CareConnect.Infrastructure.Repositories.Visits;
using MediatR;

namespace CareConnect.Commands.Visits;

public sealed class UpdateVisitCommandHandler : IRequestHandler<UpdateVisitCommand>
{
    private readonly IVisitRepository _repository;

    public UpdateVisitCommandHandler(IVisitRepository repository)
    {
        _repository = repository;
    }

    public Task Handle(UpdateVisitCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Visit;

        return _repository.RescheduleVisitAsync(request.VisitId, dto.ScheduledStartUtc, dto.ScheduledEndUtc, cancellationToken);
    }
}
