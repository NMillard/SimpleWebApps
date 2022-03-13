using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using RepositoryPattern;
using RepositoryPattern.Samples;
using Xunit;

namespace DesignPatterns.SharedTests {
    public class RepositoryPatternDemo {

        [Fact]
        public async Task AddUserExampleWithAggregateRepository() {
            DbContextOptionsBuilder dbContextOptionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase(Guid.NewGuid().ToString());
            AggregateRootRepositoryBase<User> sut = new UserRepository(new AppDbContext(dbContextOptionsBuilder.Options), new NullLogger<UserRepository>());

            var entity = new User("nicm");
            bool result = await sut.AddAsync(entity);
            
            Assert.True(result);
        }

        [Fact]
        public async Task GetUserExampleWithInterface() {
            var userId = Guid.NewGuid();
            var mock = new Mock<IUserRepository>();
            mock.Setup(u => u.GetAsync(u => u.Id == userId))
                .ReturnsAsync(new User("nicm"))
                .Verifiable();
            
            var sut = new GetUserQuery(mock.Object);

            User? result = await sut.ExecuteAsync(userId);
            
            Assert.NotNull(result);
            mock.Verify();
        }

        [Fact]
        public void GetUserExample() {
            DbContextOptionsBuilder dbContextOptionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var context = new AppDbContext(dbContextOptionsBuilder.Options);
            var repository = new UserRepository(context, new NullLogger<UserRepository>());
            
            var sut1 = new GetUserQuery(repository);
            var sut3 = new GetUserQuery(context);
        }

        [Fact]
        public void GetUserWithDependencyInjectionExample() {
            ServiceProvider provider = new ServiceCollection()
                .AddRepositories()
                .BuildServiceProvider();

            var repository = provider.GetRequiredService<IUserRepository>();

            var sut = new GetUserQuery(repository);
        }
    }
}