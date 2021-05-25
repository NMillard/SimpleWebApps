using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Polymorphism.WebApi.DataLayer;
using Polymorphism.WebApi.DataLayer.Repositories;
using Polymorphism.WebApi.Users.Commands;
using Polymorphism.WebApi.Users.Models;
using Xunit;

namespace Polymorphism.Tests {
    public class UserCommandsShould {
        private readonly IUserRepository repository;
        private readonly AppDbContext context;

        public UserCommandsShould() {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            context = new AppDbContext(optionsBuilder.Options);
            repository = new UserRepository(context);
        }
        
        
        [Theory]
        [MemberData(nameof(UserInputs))]
        public async Task CreateRegularUser(CreateUserInputBase input, bool isPremium, bool isOnTrial) {
            // Arrange
            var sut = new CreateUserCommand(repository);
            
            // Act
            bool result = await sut.ExecuteAsync(input);

            User user = await context.Users.SingleAsync();
            Assert.True(result);
            Assert.Equal(user.Username, input.Username);
            Assert.Equal(user.IsPremium, isPremium);
            Assert.Equal(user.IsOnTrial, isOnTrial);
        }

        public static IEnumerable<object[]> UserInputs() => new[] {
            new object[] { new CreateRegularUserInput { Username = "nicm" }, false, false },
            new object[] { new CreatePremiumUserInput { Username = "nicm" }, true, false },
        };
    }
}