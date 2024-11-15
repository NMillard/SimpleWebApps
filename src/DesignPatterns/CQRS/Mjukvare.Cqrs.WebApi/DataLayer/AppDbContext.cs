using Microsoft.EntityFrameworkCore;
using Mjukvare.Cqrs.WebApi.Domain;
using Mjukvare.Cqrs.WebApi.Domain.ReadModels;

namespace Mjukvare.Cqrs.WebApi.DataLayer;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Checkin> Checkins { get; set; }
    public DbSet<UserCheckinDisplay> UserCheckinDisplays { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}