using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi {
    /*
     * database contexts should always be private and in a separate project.
     */
    public class AppDbContext : DbContext {

        public AppDbContext(DbContextOptions options) : base(options) { }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Article> Articles { get; set; }
    }
}