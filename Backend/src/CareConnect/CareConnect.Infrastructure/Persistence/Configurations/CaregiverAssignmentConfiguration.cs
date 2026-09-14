using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareConnect.Infrastructure.Persistence.Configurations;

public class CaregiverAssignmentConfiguration : IEntityTypeConfiguration<CaregiverAssignment>
{
    public void Configure(EntityTypeBuilder<CaregiverAssignment> builder)
    {
        builder.ToTable("CaregiverAssignments", t =>
            t.HasCheckConstraint("CK_CaregiverAssignment_EndDate_AfterStartDate", "[EndDate] IS NULL OR [EndDate] >= [StartDate]"));

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.StartDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(a => a.EndDate)
            .HasColumnType("date");

        builder.Property(a => a.Notes)
            .HasMaxLength(500);

        builder.HasIndex(a => new { a.ClientId, a.CaregiverId });

        // Only one Active assignment allowed per caregiver/client pair at a time.
        builder.HasIndex(a => new { a.CaregiverId, a.ClientId })
            .IsUnique()
            .HasFilter("[Status] = 1")
            .HasDatabaseName("IX_CaregiverAssignments_CaregiverId_ClientId_ActiveOnly");

        builder.HasOne(a => a.Caregiver)
            .WithMany(c => c.CaregiverAssignments)
            .HasForeignKey(a => a.CaregiverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Client)
            .WithMany(c => c.CaregiverAssignments)
            .HasForeignKey(a => a.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ConfigureAuditFields();
    }
}
