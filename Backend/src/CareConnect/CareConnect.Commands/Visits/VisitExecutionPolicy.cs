namespace CareConnect.Commands.Visits;

/// <summary>Named, centrally-configured thresholds for the visit execution workflow.</summary>
internal static class VisitExecutionPolicy
{
    /// <summary>How early a caregiver may check in relative to the visit's scheduled start.</summary>
    public const int CheckInEarlyGraceMinutes = 15;
}
