using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi {
    
    public class AppDbContext : DbContext {

        public AppDbContext(DbContextOptions options) : base(options) { }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Article> Articles { get; set; }
    }
}