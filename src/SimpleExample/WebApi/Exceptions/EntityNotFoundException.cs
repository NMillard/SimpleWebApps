using System;
using WebApi.Models;

namespace WebApi.Exceptions {
    
    /*
     * Just to demo how you can make application specific exceptions.
     * Modeling your domain is not only about the entities. Great designs
     * also take into account all the things that can go wrong.
     *
     * It's more expressive to throw exceptions tailored to the application
     * than generic ones.
     */
    public class EntityNotFoundException<TEntity> : Exception where TEntity : EntityBase {
        public EntityNotFoundException(int id) : base($"{typeof(TEntity).Name} entity Id {id.ToString()} not found") {
        }
    }
}