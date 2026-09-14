using CareConnect.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Infrastructure.Persistence;

public class CareConnectDbContext : DbContext
{
    public CareConnectDbContext(DbContextOptions<CareConnectDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Caregiver> Caregivers => Set<Caregiver>();

    public DbSet<Client> Clients => Set<Client>();

    public DbSet<CareTask> CareTasks => Set<CareTask>();

    public DbSet<CaregiverAssignment> CaregiverAssignments => Set<CaregiverAssignment>();

    public DbSet<CaregiverAvailability> CaregiverAvailabilities => Set<CaregiverAvailability>();

    public DbSet<Visit> Visits => Set<Visit>();

    public DbSet<VisitTask> VisitTasks => Set<VisitTask>();

    public DbSet<VisitNote> VisitNotes => Set<VisitNote>();

    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CareConnectDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
