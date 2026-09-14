using CareConnect.DTOs.Enums;
using CareConnect.Infrastructure.Entities.Common;

namespace CareConnect.Infrastructure.Entities;

public class Invoice : IAuditable
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    public Client Client { get; set; } = null!;

    public int? VisitId { get; set; }

    public Visit? Visit { get; set; }

    public string InvoiceNumber { get; set; } = null!;

    public decimal Amount { get; set; }

    public InvoiceStatus Status { get; set; }

    public DateOnly IssuedDate { get; set; }

    public DateOnly? DueDate { get; set; }

    public DateOnly? PaidDate { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
}
