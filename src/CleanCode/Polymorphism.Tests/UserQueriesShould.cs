using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Polymorphism.WebApi.DataLayer;
using Polymorphism.WebApi.DataLayer.Repositories;
using Polymorphism.WebApi.Users.Models;
using Polymorphism.WebApi.Users.Queries;
using Xunit;

namespace Polymorphism.Tests {
    public class UserQueriesShould {
        private readonly IUserRepository repository;
        private readonly AppDbContext context;
        
        public UserQueriesShould() {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            context = new AppDbContext(optionsBuilder.Options);
            repository = new UserRepository(context);
        }

        [Fact]
        public async Task GetAllUsers() {
            // Arrange
            var users = new List<User> {
                new("nicm"),
                new("nmillard")
            };
            context.Users.AddRange(users);
            await context.SaveChangesAsync();

            var sut = new GetAllUsersQuery(repository);

            // Act
            IEnumerable<User> result = await sut.ExecuteAsync();
            
            Assert.Equal(users.Count, result.Count());
        }
    }
}