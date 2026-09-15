using CareConnect.DTOs.Assignments;
using CareConnect.DTOs.Common;
using CareConnect.DTOs.Enums;
using MediatR;

namespace CareConnect.Queries.Assignments.GetAssignments;

public sealed record GetAssignmentsQuery(
    int Page,
    int PageSize,
    AssignmentStatus? Status,
    string RequestingAuth0UserId) : IRequest<PagedResponseDto<AssignmentDto>>;
