using CareConnect.DTOs.Visits;
using MediatR;

namespace CareConnect.Commands.Visits;

/// <summary>Schedules a new visit. Returns the new visit's Id. Always starts as Scheduled.</summary>
public sealed record CreateVisitCommand(CreateVisitDto Visit) : IRequest<int>;
