using CareConnect.DTOs.Enums;
using CareConnect.Infrastructure.Entities.Common;

namespace CareConnect.Infrastructure.Entities;

public class User : IAuditable
{
    public int Id { get; set; }

    public string Auth0UserId { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public UserRole Role { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }

    public Caregiver? Caregiver { get; set; }

    public Client? Client { get; set; }
}
