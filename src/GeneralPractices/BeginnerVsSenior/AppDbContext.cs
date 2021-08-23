using Microsoft.EntityFrameworkCore;

namespace BeginnerVsSenior {
    public class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}