using System;
using System.Threading.Tasks;

namespace BeginnerVsSenior.Junior.WebAPI {
    public class User {
        public User() {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string Email { get; set; }

        public async Task<bool> SaveAsync(AppDbContext context) {
            await context.AddAsync(this);
            await context.SaveChangesAsync();

            return true;
        }
    }
}