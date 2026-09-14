using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareConnect.Infrastructure.Persistence.Configurations;

public class VisitConfiguration : IEntityTypeConfiguration<Visit>
{
    public void Configure(EntityTypeBuilder<Visit> builder)
    {
        builder.ToTable("Visits", t =>
        {
            t.HasCheckConstraint("CK_Visit_ScheduledEnd_AfterScheduledStart", "[ScheduledEndUtc] > [ScheduledStartUtc]");
            t.HasCheckConstraint(
                "CK_Visit_ActualEnd_AfterActualStart",
                "[ActualStartUtc] IS NULL OR [ActualEndUtc] IS NULL OR [ActualEndUtc] > [ActualStartUtc]");
        });

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(v => v.CancellationReason)
            .HasMaxLength(300);

        builder.HasIndex(v => new { v.CaregiverAssignmentId, v.ScheduledStartUtc });

        builder.HasIndex(v => v.Status);

        builder.HasOne(v => v.CaregiverAssignment)
            .WithMany(a => a.Visits)
            .HasForeignKey(v => v.CaregiverAssignmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ConfigureAuditFields();
    }
}
