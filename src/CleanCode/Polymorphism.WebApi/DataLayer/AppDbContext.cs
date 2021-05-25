using Microsoft.EntityFrameworkCore;
using Polymorphism.WebApi.Users.Models;

namespace Polymorphism.WebApi.DataLayer {
    public class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            modelBuilder.HasDefaultSchema("Polymorphism");
        }
    }
}