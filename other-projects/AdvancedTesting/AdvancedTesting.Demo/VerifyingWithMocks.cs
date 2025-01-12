using AdvancedTesting.Demo.WebApp.Domain;
using AdvancedTesting.Demo.WebApp.Repositories;
using AdvancedTesting.Demo.WebApp.Services;
using FluentAssertions;
using NSubstitute;

namespace AdvancedTesting.Demo;

public class VerifyingWithMocks
{
    [Fact]
    public async Task SimpleMocking()
    {
        var repository = Substitute.For<IUserRepository>();
        repository.GetUserByName("Faxe").Returns(Task.FromResult(new User()
        {
            Id = Guid.NewGuid(),
            Name = "Faxe Kondi"
        }));

        var userManager = new UserManager(repository);

        User? result = await userManager.GetUser("Faxe");

        result.Should().NotBeNull();
    }

    [Fact]
    public void ShouldSaveUser()
    {
        // Arrange
        var repository = Substitute.For<IUserRepository>();

        var user = new User();
        var sut = new UserManager(repository);
        
        // Act
        sut.SaveUser(user);
        
        // Assert
        repository.Received(1).SaveUser(Arg.Any<User>());
    }

    [Fact]
    public void ValidateMockInteraction()
    {
        var repository = Substitute.For<IUserRepository>();
        var result = new List<object>();
        repository.When(r => r.SaveUser(Arg.Any<User>()))
            .Do(info => result.Add(info.ArgAt<User>(0)));

        var userManager = new UserManager(repository);

        userManager.SaveUsers(new User { Name = "Nimi" }, new User { Name = "Ems" });
        
        repository.Received(2).SaveUser(Arg.Any<User>());
        repository.Received(1).SaveUser(Arg.Is<User>(o => o.Name.Equals("Nimi")));
        repository.Received(1).SaveUser(Arg.Is<User>(o => o.Name.Equals("Ems")));
        
        result.OfType<User>()
            .Should()
            .HaveCount(2).And
            .Satisfy(
                e => e.Name.Equals("Nimi"),
                e => e.Name.Equals("Ems")
            );
    }
}