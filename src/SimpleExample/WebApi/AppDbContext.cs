using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApi.Models;

namespace WebApi {
    /*
     * database contexts should always be private and in a separate project.
     */
    public class AppDbContext : DbContext {
        private readonly IConfiguration configuration;

        public AppDbContext(IConfiguration configuration) {
            this.configuration = configuration;
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Article> Articles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            // Configuring your sql connection like this provides no flexibility.
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("default"));
        }
    }
}