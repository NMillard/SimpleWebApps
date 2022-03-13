using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RepositoryPattern.Samples {
    internal class RegisterUserCommand {
        private readonly IUserRepository repository;

        public RegisterUserCommand(IUserRepository repository) {
            this.repository = repository;
        }

        public async Task<bool> ExecuteAsync(User user) {
            if (user is null) throw new InvalidOperationException();
            
            return await repository.AddAsync(user);
        }
    }

    internal class GetUserQuery {
        private readonly AppDbContext context;
        private readonly IRepository<User> repository;

        public GetUserQuery(IRepository<User> repository) {
            this.repository = repository;
        }

        // Show the downsides of this approach.
        public GetUserQuery(AppDbContext context) {
            this.context = context;
        }

        public async Task<User?> ExecuteAsync(Guid userId)
            => await repository.GetAsync(u => u.Id == userId);

        public async Task<User?> ExecuteFromContext(Guid userId)
            => await context.Users
                .Include(u => u.Books)
                .SingleOrDefaultAsync(u => u.Id == userId);
    }
}