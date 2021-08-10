using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrudApp.Domain;
using CrudApp.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CrudApp.Features.Chat {
    public class ChatRepository : CrudRepository<ChatChannel, Guid> {
        public ChatRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<ChatChannel>> GetAsync() => await Entities.ToListAsync();
    }
}