using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareConnect.Infrastructure.Persistence.Configurations;

public class CaregiverConfiguration : IEntityTypeConfiguration<Caregiver>
{
    public void Configure(EntityTypeBuilder<Caregiver> builder)
    {
        builder.ToTable("Caregivers", t =>
        {
            t.HasCheckConstraint("CK_Caregiver_HourlyRate_Positive", "[HourlyRate] > 0");
            t.HasCheckConstraint("CK_Caregiver_YearsOfExperience_NonNegative", "[YearsOfExperience] >= 0");
            t.HasCheckConstraint("CK_Caregiver_DateOfBirth_NotInFuture", "[DateOfBirth] <= CAST(SYSUTCDATETIME() AS date)");
        });

        builder.HasKey(c => c.Id);

        builder.Property(c => c.LicenseNumber)
            .HasMaxLength(50);

        builder.Property(c => c.HourlyRate)
            .IsRequired()
            .HasPrecision(10, 2);

        builder.Property(c => c.HireDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(c => c.DateOfBirth)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(c => c.YearsOfExperience)
            .IsRequired();

        builder.Property(c => c.IsActive)
            .IsRequired();

        builder.HasIndex(c => c.UserId)
            .IsUnique();

        builder.HasOne(c => c.User)
            .WithOne(u => u.Caregiver)
            .HasForeignKey<Caregiver>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ConfigureAuditFields();
    }
}
