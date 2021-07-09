using System;
using System.Threading.Tasks;
using CrudApp.Domain;
using Microsoft.EntityFrameworkCore;

namespace CrudApp.Persistence {
    /// <summary>
    /// Generic repository providing a basic set of CRUD operations on the entity.
    /// </summary>
    public class CrudRepository<TEntity, TId> where TEntity : class, IEntity<TId> {
        private readonly AppDbContext context;

        public CrudRepository(AppDbContext context) {
            this.context = context;
            Entities = context.Set<TEntity>();
        }

        protected DbSet<TEntity> Entities { get; }

        public async Task<TEntity?> GetAsync(TId id) => await Entities.SingleOrDefaultAsync(e => e.Id!.Equals(id));

        public async Task<bool> AddAsync(TEntity entity) {
            await Entities.AddAsync(entity);
            bool result = await SaveAsync();
            
            return result;
        }

        public async Task<bool> UpdateAsync(TEntity entity) {
            Entities.Update(entity);
            bool result = await SaveAsync();

            return result;
        }

        public async Task<bool> DeleteAsync(TEntity entity) {
            Entities.Remove(entity);
            bool result = await SaveAsync();

            return result;
        }

        private async Task<bool> SaveAsync() {
            try {
                await context.SaveChangesAsync();
                return true;
            } catch (DbUpdateException due) {
                Console.WriteLine(due);
                return false;
            }
        }
    }
}