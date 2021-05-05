using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Repositories {
    
    /*
     * Generic repository base classes are likely unnecessary and forces
     * your subclasses to implement behavior that is not needed.
     * A repository should, in my mind, be task-oriented and not
     * CRUD-based.
     *
     * Naming members of a base class also becomes too generic to be useful
     * anywhere.
     */
    
    public abstract class RepositoryBase<T> where T : EntityBase {
        private readonly AppDbContext context;
        protected readonly DbSet<T> Set;

        protected RepositoryBase(AppDbContext context) {
            this.context = context;
            Set = context.Set<T>();
        }

        public virtual T Get(int id) {
            return Set.SingleOrDefault(e => e.Id.Equals(id));
        }

        public virtual List<T> All() {
            return Set.ToList();
        }
        
        public abstract void Create(T entity);
        public abstract void Delete(int id);

        /*
         * Not catching exceptions will bite you in the A...
         */
        protected void SaveChanges() {
            context.SaveChanges();
        }
    }
}