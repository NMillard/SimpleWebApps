using System;
using CrudApp.Domain;
using CrudApp.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CrudApp.Features.Authors {
    public class AuthorRepository : CrudRepository<Author, Guid> {
        public AuthorRepository(AppDbContext context) : base(context) { }
    }
}