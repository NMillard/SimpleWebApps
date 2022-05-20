using Microsoft.EntityFrameworkCore;
using SchedulerAgentSupervisor.WebApi.Domain;

namespace SchedulerAgentSupervisor.WebApi.Persistence;

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions options) : base(options) { }
    
    public DbSet<Schedule> Schedules { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}