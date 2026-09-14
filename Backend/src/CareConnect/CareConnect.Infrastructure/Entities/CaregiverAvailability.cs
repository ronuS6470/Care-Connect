using CareConnect.Infrastructure.Entities.Common;

namespace CareConnect.Infrastructure.Entities;

public class CaregiverAvailability : IAuditable
{
    public int Id { get; set; }

    public int CaregiverId { get; set; }

    public Caregiver Caregiver { get; set; } = null!;

    public DayOfWeek DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
}
