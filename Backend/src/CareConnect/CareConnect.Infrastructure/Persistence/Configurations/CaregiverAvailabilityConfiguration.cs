using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareConnect.Infrastructure.Persistence.Configurations;

public class CaregiverAvailabilityConfiguration : IEntityTypeConfiguration<CaregiverAvailability>
{
    public void Configure(EntityTypeBuilder<CaregiverAvailability> builder)
    {
        builder.ToTable("CaregiverAvailabilities", t =>
            t.HasCheckConstraint("CK_CaregiverAvailability_EndTime_AfterStartTime", "[EndTime] > [StartTime]"));

        builder.HasKey(a => a.Id);

        builder.Property(a => a.DayOfWeek)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.StartTime)
            .IsRequired()
            .HasColumnType("time");

        builder.Property(a => a.EndTime)
            .IsRequired()
            .HasColumnType("time");

        builder.Property(a => a.IsActive)
            .IsRequired();

        builder.HasIndex(a => new { a.CaregiverId, a.DayOfWeek, a.StartTime })
            .IsUnique();

        builder.HasOne(a => a.Caregiver)
            .WithMany(c => c.CaregiverAvailabilities)
            .HasForeignKey(a => a.CaregiverId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ConfigureAuditFields();
    }
}
