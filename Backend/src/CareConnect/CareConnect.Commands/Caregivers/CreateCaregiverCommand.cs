using CareConnect.DTOs.Caregivers;
using MediatR;

namespace CareConnect.Commands.Caregivers;

/// <summary>Creates a caregiver profile for an existing user. Returns the new caregiver's Id.</summary>
public sealed record CreateCaregiverCommand(CreateCaregiverDto Caregiver) : IRequest<int>;
