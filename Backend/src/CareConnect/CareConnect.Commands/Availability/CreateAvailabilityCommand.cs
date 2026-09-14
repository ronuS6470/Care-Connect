using CareConnect.DTOs.Caregivers;
using MediatR;

namespace CareConnect.Commands.Availability;

public sealed record CreateAvailabilityCommand(CreateCaregiverAvailabilityDto Availability) : IRequest<int>;
