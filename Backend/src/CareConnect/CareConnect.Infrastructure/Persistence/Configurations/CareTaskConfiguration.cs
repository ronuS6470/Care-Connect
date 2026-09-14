using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareConnect.Infrastructure.Persistence.Configurations;

public class CareTaskConfiguration : IEntityTypeConfiguration<CareTask>
{
    public void Configure(EntityTypeBuilder<CareTask> builder)
    {
        builder.ToTable("CareTasks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.Property(t => t.IsActive)
            .IsRequired();

        builder.HasIndex(t => t.Name)
            .IsUnique();

        builder.ConfigureAuditFields();
    }
}
