using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RepositoryPattern {
    public interface IRepository<TEntity> where TEntity : class {
        public Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate);
        public Task<IEnumerable<TEntity>> GetAllAsync();
        public Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);
        public Task<bool> AddAsync(TEntity entity);
        public Task<bool> UpdateAsync(TEntity entity);
    }
    
    /// <summary>
    /// Internal repository base class that shouldn't be exposed to the outside world since class
    /// contains implementation details.
    /// </summary>
    internal abstract class AggregateRootRepositoryBase<TEntity> : IRepository<TEntity> where TEntity : class {
        protected readonly DbContext Context;
        protected event EventHandler<SavingEventArgs<TEntity>>? BeforeSave;
        protected event EventHandler<SavingEventArgs<TEntity>>? AfterSave;
        protected event EventHandler<SavingFailedEventArgs<TEntity>>? SaveFailed;
        
        protected AggregateRootRepositoryBase(DbContext context) {
            Context = context;
            Entities = context.Set<TEntity>().AsNoTracking().AsQueryable();
        }
        
        protected IQueryable<TEntity> Entities { get; init; }

        public virtual async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate) 
            => await Entities.SingleOrDefaultAsync(predicate);

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
            => await Entities.Where(predicate).ToListAsync();

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync() => await Entities.ToListAsync();
        
        public virtual async Task<bool> AddAsync(TEntity entity) {
            await Context.AddAsync(entity);
            bool result = await SaveAsync(entity);

            return result;
        }

        public virtual async Task<bool> UpdateAsync(TEntity entity) {
            Context.Update(entity);
            bool result = await SaveAsync(entity);

            return result;
        }

        protected virtual async Task<bool> SaveAsync(TEntity entity) {
            BeforeSave?.Invoke(this, new SavingEventArgs<TEntity>(entity));

            try {
                await Context.SaveChangesAsync();
                AfterSave?.Invoke(this, new SavingEventArgs<TEntity>(entity));

                return true;
            } catch (DbUpdateException due) {
                SaveFailed?.Invoke(this, new SavingFailedEventArgs<TEntity>(entity, due));
                return false;
            }
        }
    }
}