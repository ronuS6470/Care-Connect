using CareConnect.Infrastructure.Entities.Common;

namespace CareConnect.Infrastructure.Entities;

public class Caregiver : IAuditable
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public string? LicenseNumber { get; set; }

    public decimal HourlyRate { get; set; }

    public DateOnly HireDate { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public int YearsOfExperience { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }

    public ICollection<CaregiverAssignment> CaregiverAssignments { get; set; } = new List<CaregiverAssignment>();

    public ICollection<CaregiverAvailability> CaregiverAvailabilities { get; set; } = new List<CaregiverAvailability>();
}
