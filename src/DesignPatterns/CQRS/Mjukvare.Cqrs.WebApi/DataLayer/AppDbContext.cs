using Microsoft.EntityFrameworkCore;
using Mjukvare.Cqrs.WebApi.Domain;

namespace Mjukvare.Cqrs.WebApi.DataLayer;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options) { }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Checkin> Checkins { get; set; }
}