using CrudApp.Domain;
using Microsoft.EntityFrameworkCore;

namespace CrudApp.Persistence {
    public class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions options) : base(options) { }
        
        public DbSet<Author> Authors { get; set; }
    }
}