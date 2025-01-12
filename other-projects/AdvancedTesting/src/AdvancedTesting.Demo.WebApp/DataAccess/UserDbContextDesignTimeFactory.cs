using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AdvancedTesting.Demo.WebApp.DataAccess;

public class UserDbContextDesignTimeFactory: IDesignTimeDbContextFactory<UserDbContext>
{
    public UserDbContextDesignTimeFactory() { }

    public UserDbContextDesignTimeFactory(string connectionString)
        => ConnectionString = connectionString;

    public string? ConnectionString { get; set; }
    
    public UserDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        string connection = ConnectionString
                            ?? config.GetConnectionString("Postgres")
                            ?? throw new InvalidOperationException();
        
        DbContextOptionsBuilder dbContextOptionsBuilder = new DbContextOptionsBuilder()
            .UseNpgsql(connection);
        
        return new UserDbContext(dbContextOptionsBuilder.Options);
    }
}