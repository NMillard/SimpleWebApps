using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebApi.Exceptions;
using WebApi.Models;

namespace WebApi.Repositories {
    
    /*
     * Like the EntityBase class, generic repositories may be okay
     * for beginners.
     * 
     * But, generic repository base classes are likely unnecessary and forces
     * your subclasses to implement behavior that is not needed.
     * A repository should, in my mind, be task-oriented and not
     * CRUD-based.
     *
     * Naming members of a base class also becomes too generic to be useful
     * anywhere.
     */
    
    public abstract class RepositoryBase<T> where T : EntityBase {
        private readonly AppDbContext context;
        protected readonly DbSet<T> Entities;

        protected RepositoryBase(AppDbContext context) {
            this.context = context;
            Entities = context.Set<T>();
        }

        public virtual T Get(int id) {
            T entity = Entities.SingleOrDefault(e => e.Id.Equals(id));
            if (entity is null) throw new EntityNotFoundException<T>(id);
            
            return entity;
        }

        public virtual List<T> All() {
            return Entities.ToList();
        }

        public virtual void Update(T entity) {
            Entities.Update(entity);
            SaveChanges();
        }
        
        public virtual int Create(T entity) {
            Entities.Add(entity);
            SaveChanges();

            return entity.Id;
        }

        public virtual void Delete(int id) {
            T entity = Get(id);
            Entities.Remove(entity);

            SaveChanges();
        }

        protected bool SaveChanges() {
            try {
                context.SaveChanges();
                return true;
            } catch (Exception) {
                return false;
            }
        }
    }
}