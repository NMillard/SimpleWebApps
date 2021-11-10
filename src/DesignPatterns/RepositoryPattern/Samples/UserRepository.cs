using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;

namespace RepositoryPattern.Samples {

    public interface IUserRepository : IRepository<User> {
        // Task<User> GetWithBooks(Guid id);
        // Task<User> GetWithBooksAndFavoriteFoods(Guid id);
        // Task<User> GetWithBooksAndFavoriteFoodsAndPets(Guid id);
        // omitted CRUD operations for brevity
    }

    internal sealed class UserRepository : AggregateRootRepositoryBase<User>, IUserRepository {
        public UserRepository(AppDbContext context, ILogger<UserRepository> logger) : base(context) {
            Entities = context.Set<User>().Include(u => u.Books);

            BeforeSave += (sender, args) => logger.LogInformation("Saving user {id}", args.Entity.Id);
        }
    }
}