using AdvancedTesting.Demo.WebApp.DataAccess;
using AdvancedTesting.Demo.WebApp.Domain;
using AdvancedTesting.Demo.WebApp.Repositories;
using AdvancedTesting.Demo.WebApp.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Testcontainers.PostgreSql;

namespace AdvancedTesting.Demo;

public class ContainerizedTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    [Fact]
    public async Task UsingTestContainer()
    {
        UserDbContext context = new TestDesignTimeFactory(fixture.ConnectionString).CreateDbContext(null!);
        await context.Database.EnsureCreatedAsync();
        await context.Users.ExecuteDeleteAsync();

        context.Users.Add(new User { Name = "Nimi" });
        await context.SaveChangesAsync();

        List<User> result = context.Users
            .AsNoTracking()
            .ToList();
        
        result.Should().HaveCount(1).And
            .Satisfy(u => u.Name.Equals("Nimi"));
    }
    
    [Fact]
    public async Task VerifyUserDataIntegrity()
    {
        // Arrange
        UserDbContext context = new TestDesignTimeFactory(fixture.ConnectionString).CreateDbContext(null!);
        await context.Database.EnsureCreatedAsync();
        await context.Users.ExecuteDeleteAsync();

        var repository = new EfUserRepository(context);
        
        var user = new User();
        var sut = new UserManager(repository);

        // Act
        sut.SaveUser(user);
        
        // Assert
        List<User> result = context.Users
            .AsNoTracking()
            .ToList();
        
        result.Should().HaveCount(1).And
            .Satisfy(u => u.Name.Equals("Nimi"));
    }
    
    [Fact]
    public async Task Demo3()
    {
        UserDbContext context = new TestDesignTimeFactory(fixture.ConnectionString).CreateDbContext(null!);
        await context.Database.EnsureCreatedAsync();
        await context.Users.ExecuteDeleteAsync();

        context.Users.Add(new User { Name = "Ems" });
        await context.SaveChangesAsync();

        List<User> result = context.Users
            .AsNoTracking()
            .ToList();

        result.Should().HaveCount(1).And
            .Satisfy(u => u.Name.Equals("Ems"));
    }
}


public class Demo3
{
    [Fact]
    public void MakeHttpRequest()
    {
        
    }
}

public class TestDesignTimeFactory : IDesignTimeDbContextFactory<UserDbContext>
{
    public TestDesignTimeFactory(string connectionString)
        => ConnectionString = connectionString;

    private string? ConnectionString { get; }
    
    public UserDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder dbContextOptionsBuilder = new DbContextOptionsBuilder()
            .UseNpgsql(ConnectionString);
        
        return new UserDbContext(dbContextOptionsBuilder.Options);
    }
}

public class DatabaseFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer container = new PostgreSqlBuilder()
        .Build();

    public string ConnectionString => container.GetConnectionString();

    public Task InitializeAsync()
    {
        return container.StartAsync();
    }

    public Task DisposeAsync()
    {
        return container.DisposeAsync().AsTask();
    }
}