using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareConnect.Infrastructure.Persistence.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices", t =>
        {
            t.HasCheckConstraint("CK_Invoice_Amount_NonNegative", "[Amount] >= 0");
            t.HasCheckConstraint("CK_Invoice_DueDate_OnOrAfterIssuedDate", "[DueDate] IS NULL OR [DueDate] >= [IssuedDate]");
        });

        builder.HasKey(i => i.Id);

        builder.Property(i => i.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(i => i.Amount)
            .IsRequired()
            .HasPrecision(10, 2);

        builder.Property(i => i.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(i => i.IssuedDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(i => i.DueDate)
            .HasColumnType("date");

        builder.Property(i => i.PaidDate)
            .HasColumnType("date");

        builder.HasIndex(i => i.InvoiceNumber)
            .IsUnique();

        builder.HasIndex(i => i.VisitId)
            .IsUnique()
            .HasFilter("[VisitId] IS NOT NULL");

        builder.HasIndex(i => new { i.ClientId, i.Status });

        builder.HasOne(i => i.Client)
            .WithMany(c => c.Invoices)
            .HasForeignKey(i => i.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Visit)
            .WithOne(v => v.Invoice)
            .HasForeignKey<Invoice>(i => i.VisitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ConfigureAuditFields();
    }
}
