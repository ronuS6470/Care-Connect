using CareConnect.AppServices.Security;
using CareConnect.Commands.Assignments;
using CareConnect.DTOs.Assignments;
using CareConnect.DTOs.Common;
using CareConnect.DTOs.Enums;
using CareConnect.Queries.Assignments.GetAssignmentById;
using CareConnect.Queries.Assignments.GetAssignments;
using MediatR;

namespace CareConnect.AppServices.Assignments;

public sealed class AssignmentsAppService : IAssignmentsAppService
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public AssignmentsAppService(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    {
        _mediator = mediator;
        _currentUserAccessor = currentUserAccessor;
    }

    public Task<PagedResponseDto<AssignmentDto>> GetAssignmentsAsync(
        int page, int pageSize, AssignmentStatus? status, CancellationToken cancellationToken) =>
        _mediator.Send(new GetAssignmentsQuery(page, pageSize, status, _currentUserAccessor.Auth0UserId), cancellationToken);

    public Task<AssignmentDto?> GetAssignmentByIdAsync(int assignmentId, CancellationToken cancellationToken) =>
        _mediator.Send(new GetAssignmentByIdQuery(assignmentId, _currentUserAccessor.Auth0UserId), cancellationToken);

    public Task<int> CreateAssignmentAsync(CreateAssignmentDto dto, CancellationToken cancellationToken) =>
        _mediator.Send(new CreateAssignmentCommand(dto), cancellationToken);

    public Task UpdateAssignmentAsync(int assignmentId, UpdateAssignmentDto dto, CancellationToken cancellationToken) =>
        _mediator.Send(new UpdateAssignmentCommand(assignmentId, dto), cancellationToken);

    public Task CancelAssignmentAsync(int assignmentId, CancellationToken cancellationToken) =>
        _mediator.Send(new CancelAssignmentCommand(assignmentId), cancellationToken);
}
