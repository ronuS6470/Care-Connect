using CareConnect.DTOs.Caregivers;
using MediatR;

namespace CareConnect.Commands.Caregivers;

public sealed record UpdateCaregiverCommand(int CaregiverId, UpdateCaregiverDto Caregiver) : IRequest;
