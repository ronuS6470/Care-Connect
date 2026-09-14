using MediatR;

namespace CareConnect.Commands.Availability;

/// <summary>
/// Removes an availability slot. Unlike Caregiver/Client/CareTask, nothing else in the schema
/// references CaregiverAvailability rows, so a physical delete is safe here — no historical
/// records depend on this being kept around.
/// </summary>
public sealed record DeleteAvailabilityCommand(int AvailabilityId) : IRequest;
