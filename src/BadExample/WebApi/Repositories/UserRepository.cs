using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Repositories {
    /*
     * Only dropping the prefixed "I" when naming a concrete class
     * of an interface is poor practice.
     *
     * Use more descriptive naming like "SqlUserRepository" or "InMemoryUserRepository" etc.
     */
    public class UserRepository : RepositoryBase<User> {
        public UserRepository(AppDbContext context) : base(context) { }

        public override User Get(int id) {
            return Set.Include(user => user.Articles)
                .SingleOrDefault(user => user.Id == id);
        }

        public override void Create(User entity) {
            Set.Add(entity);
            SaveChanges();
        }

        public override void Delete(int id) {
            User user = Get(id);
            Set.Remove(user);

            SaveChanges();
        }
    }
}