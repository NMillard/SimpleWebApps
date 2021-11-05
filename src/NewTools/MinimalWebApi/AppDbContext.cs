using Microsoft.EntityFrameworkCore;

namespace MinimalWebApi; 

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions options) : base(options) { }
}