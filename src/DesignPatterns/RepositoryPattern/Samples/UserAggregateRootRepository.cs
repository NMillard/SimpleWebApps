using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RepositoryPattern.Samples {

    public class User {
        public Guid Id { get; set; }
        public List<Book> Books { get; set; }
    }

    public class Book {
        
    }

    public class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions options) : base(options){
        }

        public DbSet<User> Users { get; set; }
    }
    
    public sealed class UserRepository : AggregateRootRepositoryBase<User> {
        public UserRepository(AppDbContext context) : base(context) {
            Entities = context.Set<User>().Include(u => u.Books);
        }
    }
}