using CrudApp.Domain;
using Microsoft.EntityFrameworkCore;

namespace CrudApp.Persistence {
    public class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions options) : base(options) { }
        
        public DbSet<Author> Authors { get; set; }
        public DbSet<ChatChannel> Channels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}