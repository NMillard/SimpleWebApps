using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Reconstitution.Demo;
using Xunit.Abstractions;

namespace Reconstitution.Prototyping;

public class Prototyping
{
    private readonly ITestOutputHelper testOutputHelper;
    private readonly DbContextOptionsBuilder builder;

    public Prototyping(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
        IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        builder = new DbContextOptionsBuilder().UseSqlServer(configuration.GetConnectionString("Sql"));
    }
    
    [Fact]
    public void Test1()
    {
        var db = new AppDbContext(builder.Options);
        List<User> users = db.Users.Include(u => u.Claims).ToList();
    }

    [Fact]
    public async void Test2()
    {
        var db = new AppDbContext(builder.Options);
        var query = new GetUserQuery(db);

        User? user = await query.ExecuteAsync(Guid.Parse("4F55812A-CD28-4F48-A68B-23BB7702F358"));
    }

    [Fact]
    public async Task Test3()
    {
        var db = new AppDbContext(builder.Options);
        var query = new GetUsersQuery(db);

        await foreach (User user in query.ExecuteAsync())
        {
            testOutputHelper.WriteLine(user.Username);
        }
    }
}