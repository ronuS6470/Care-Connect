using CareConnect.Infrastructure.Entities.Common;

namespace CareConnect.Infrastructure.Entities;

public class VisitNote : IAuditable
{
    public int Id { get; set; }

    public int VisitId { get; set; }

    public Visit Visit { get; set; } = null!;

    public int AuthorUserId { get; set; }

    public User AuthorUser { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
}
