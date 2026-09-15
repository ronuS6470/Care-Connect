using CareConnect.AppServices.Security;
using CareConnect.Commands.Visits;
using CareConnect.DTOs.Common;
using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Reporting;
using CareConnect.DTOs.Visits;
using CareConnect.Queries.Visits;
using MediatR;

namespace CareConnect.AppServices.Visits;

public sealed class VisitsAppService : IVisitsAppService
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public VisitsAppService(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    {
        _mediator = mediator;
        _currentUserAccessor = currentUserAccessor;
    }

    public Task<PagedResponseDto<VisitSummaryDto>> GetVisitsAsync(
        int page, int pageSize, DateOnly? fromDate, DateOnly? toDate, int? caregiverId, int? clientId,
        VisitStatus? status, string? search, CancellationToken cancellationToken) =>
        _mediator.Send(
            new GetVisitsQuery(page, pageSize, fromDate, toDate, caregiverId, clientId, status, search, _currentUserAccessor.Auth0UserId),
            cancellationToken);

    public Task<IReadOnlyList<VisitSummaryDto>> GetTodaysVisitsAsync(CancellationToken cancellationToken) =>
        _mediator.Send(new GetTodaysVisitsQuery(_currentUserAccessor.Auth0UserId), cancellationToken);

    public Task<PagedResponseDto<VisitSummaryDto>> GetUpcomingVisitsAsync(int page, int pageSize, CancellationToken cancellationToken) =>
        _mediator.Send(new GetUpcomingVisitsQuery(page, pageSize, _currentUserAccessor.Auth0UserId), cancellationToken);

    public Task<VisitDto?> GetVisitByIdAsync(int visitId, CancellationToken cancellationToken) =>
        _mediator.Send(new GetVisitByIdQuery(visitId, _currentUserAccessor.Auth0UserId), cancellationToken);

    public Task<int> CreateVisitAsync(CreateVisitDto dto, CancellationToken cancellationToken) =>
        _mediator.Send(new CreateVisitCommand(dto), cancellationToken);

    public Task UpdateVisitAsync(int visitId, UpdateVisitDto dto, CancellationToken cancellationToken) =>
        _mediator.Send(new UpdateVisitCommand(visitId, dto), cancellationToken);

    public Task CancelVisitAsync(int visitId, CancelVisitDto dto, CancellationToken cancellationToken) =>
        _mediator.Send(new CancelVisitCommand(visitId, dto), cancellationToken);

    public Task CheckInAsync(int visitId, CancellationToken cancellationToken) =>
        _mediator.Send(new CheckInVisitCommand(visitId, _currentUserAccessor.Auth0UserId), cancellationToken);

    public Task CheckOutAsync(int visitId, CancellationToken cancellationToken) =>
        _mediator.Send(new CheckOutVisitCommand(visitId, _currentUserAccessor.Auth0UserId), cancellationToken);

    public Task CompleteAsync(int visitId, CompleteVisitDto dto, CancellationToken cancellationToken) =>
        _mediator.Send(new CompleteVisitCommand(visitId, _currentUserAccessor.Auth0UserId, dto), cancellationToken);

    public Task<IReadOnlyList<VisitTaskDto>> GetVisitTasksAsync(int visitId, CancellationToken cancellationToken) =>
        _mediator.Send(new GetVisitTasksQuery(visitId, _currentUserAccessor.Auth0UserId), cancellationToken);

    public Task UpdateVisitTaskAsync(int visitId, int taskId, UpdateVisitTaskDto dto, CancellationToken cancellationToken) =>
        _mediator.Send(new UpdateVisitTaskCommand(visitId, taskId, dto, _currentUserAccessor.Auth0UserId), cancellationToken);

    public Task CompleteVisitTaskAsync(int visitId, int taskId, CancellationToken cancellationToken) =>
        _mediator.Send(new CompleteVisitTaskCommand(visitId, taskId, _currentUserAccessor.Auth0UserId), cancellationToken);

    public Task UncompleteVisitTaskAsync(int visitId, int taskId, CancellationToken cancellationToken) =>
        _mediator.Send(new UncompleteVisitTaskCommand(visitId, taskId, _currentUserAccessor.Auth0UserId), cancellationToken);
}
