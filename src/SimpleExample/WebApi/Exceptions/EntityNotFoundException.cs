using System;
using WebApi.Models;

namespace WebApi.Exceptions {
    public class EntityNotFoundException<TEntity> : Exception where TEntity : EntityBase {
        public EntityNotFoundException(int id) : base($"{typeof(TEntity).Name} entity Id {id.ToString()} not found") {
        }
    }
}