using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RepositoryPattern {
    public abstract class AggregateRootRepositoryBase<TEntity> where TEntity : class {
        private readonly DbContext context;
        protected event EventHandler<SavingEventArgs<TEntity>>? BeforeSave;
        protected event EventHandler<SavingEventArgs<TEntity>>? AfterSave;
        protected event EventHandler<SavingFailedEventArgs<TEntity>>? SaveFailed;
        
        protected AggregateRootRepositoryBase(DbContext context) {
            this.context = context;
            Entities = context.Set<TEntity>().AsNoTracking().AsQueryable();
        }
        
        protected IQueryable<TEntity> Entities { get; init; }

        public virtual async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate) 
            => await Entities.SingleOrDefaultAsync(predicate);

        public async Task<bool> AddAsync(TEntity entity) {
            await context.AddAsync(entity);
            bool result = await SaveAsync(entity);

            return result;
        }

        public async Task<bool> UpdateAsync(TEntity entity) {
            context.Update(entity);
            bool result = await SaveAsync(entity);

            return result;
        }

        protected virtual async Task<bool> SaveAsync(TEntity entity) {
            BeforeSave?.Invoke(this, new SavingEventArgs<TEntity>(entity));

            try {
                await context.SaveChangesAsync();
                AfterSave?.Invoke(this, new SavingEventArgs<TEntity>(entity));

                return true;
            } catch (DbUpdateException due) {
                SaveFailed?.Invoke(this, new SavingFailedEventArgs<TEntity>(entity, due));
                return false;
            }
        }
    }
}