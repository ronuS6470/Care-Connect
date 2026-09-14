using CareConnect.DTOs.Enums;
using CareConnect.Infrastructure.Entities.Common;

namespace CareConnect.Infrastructure.Entities;

public class CaregiverAssignment : IAuditable
{
    public int Id { get; set; }

    public int CaregiverId { get; set; }

    public Caregiver Caregiver { get; set; } = null!;

    public int ClientId { get; set; }

    public Client Client { get; set; } = null!;

    public AssignmentStatus Status { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }

    public ICollection<Visit> Visits { get; set; } = new List<Visit>();
}
