using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareConnect.Infrastructure.Persistence.Configurations;

public class VisitTaskConfiguration : IEntityTypeConfiguration<VisitTask>
{
    public void Configure(EntityTypeBuilder<VisitTask> builder)
    {
        builder.ToTable("VisitTasks");

        builder.HasKey(vt => vt.Id);

        builder.Property(vt => vt.IsCompleted)
            .IsRequired();

        builder.Property(vt => vt.Notes)
            .HasMaxLength(300);

        builder.HasIndex(vt => new { vt.VisitId, vt.CareTaskId })
            .IsUnique();

        builder.HasOne(vt => vt.Visit)
            .WithMany(v => v.VisitTasks)
            .HasForeignKey(vt => vt.VisitId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(vt => vt.CareTask)
            .WithMany(t => t.VisitTasks)
            .HasForeignKey(vt => vt.CareTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ConfigureAuditFields();
    }
}
