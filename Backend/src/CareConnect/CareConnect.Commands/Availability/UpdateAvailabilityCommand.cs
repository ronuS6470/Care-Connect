using CareConnect.DTOs.Caregivers;
using MediatR;

namespace CareConnect.Commands.Availability;

public sealed record UpdateAvailabilityCommand(int AvailabilityId, UpdateCaregiverAvailabilityDto Availability) : IRequest;
