using Microsoft.EntityFrameworkCore;

namespace DataLayer {
    public class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions options) : base(options) {
        }
    }
}