namespace CareConnect.AppServices.Caregivers;

/// <summary>
/// Shared by Delete/Deactivate/Activate caregiver commands — all three are the same operation
/// (toggle IsActive) with the same existence check, so it lives in one place instead of three.
/// </summary>
public interface ICaregiverActivationService
{
    Task SetActiveStatusAsync(int caregiverId, bool isActive, CancellationToken cancellationToken);
}
