using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Polymorphism.WebApi.Users.Models;

namespace Polymorphism.WebApi.DataLayer.Repositories {
    public interface IUserRepository {
        Task<bool> CreateAsync(User user);
        Task<IEnumerable<User>> GetAllAsync();
    }

    internal class UserRepository : IUserRepository {
        private readonly AppDbContext context;

        public UserRepository(AppDbContext context) {
            this.context = context;
        }
        
        public async Task<bool> CreateAsync(User user) {
            try {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
                
                return true;
            } catch (DbUpdateException) {
                return false;
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync() => await context.Users.ToListAsync();
    }
}