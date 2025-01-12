using AdvancedTesting.Demo.WebApp.Domain;
using Microsoft.EntityFrameworkCore;

namespace AdvancedTesting.Demo.WebApp.DataAccess;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
}