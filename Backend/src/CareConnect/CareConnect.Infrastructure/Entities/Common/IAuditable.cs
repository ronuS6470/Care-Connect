namespace CareConnect.Infrastructure.Entities.Common;

public interface IAuditable
{
    DateTime CreatedAtUtc { get; set; }

    DateTime? UpdatedAtUtc { get; set; }
}
