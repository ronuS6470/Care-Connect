using CareConnect.DTOs.Enums;
using CareConnect.Infrastructure.Entities.Common;

namespace CareConnect.Infrastructure.Entities;

public class Visit : IAuditable
{
    public int Id { get; set; }

    public int CaregiverAssignmentId { get; set; }

    public CaregiverAssignment CaregiverAssignment { get; set; } = null!;

    public DateTime ScheduledStartUtc { get; set; }

    public DateTime ScheduledEndUtc { get; set; }

    public DateTime? ActualStartUtc { get; set; }

    public DateTime? ActualEndUtc { get; set; }

    public VisitStatus Status { get; set; }

    public string? CancellationReason { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }

    public ICollection<VisitTask> VisitTasks { get; set; } = new List<VisitTask>();

    public ICollection<VisitNote> VisitNotes { get; set; } = new List<VisitNote>();

    public Invoice? Invoice { get; set; }
}
