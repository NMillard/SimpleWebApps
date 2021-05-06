using WebApi.Exceptions;
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
            var user = base.Get(id);
            Entities.Attach(user).Collection(u => u.Articles).Load();
            
            return user;
        }
    }
}