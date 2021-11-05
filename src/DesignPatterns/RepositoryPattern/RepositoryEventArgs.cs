using System;

namespace RepositoryPattern {
    public class SavingEventArgs<TEntity> : EventArgs where TEntity : class {
        public SavingEventArgs(TEntity entity) => Entity = entity;
        public TEntity Entity { get; }
    }
    
    public class SavingFailedEventArgs<TEntity> : EventArgs where TEntity : class {
        public SavingFailedEventArgs(TEntity entity, Exception exception) {
            Entity = entity;
            Exception = exception;
        }

        public TEntity Entity { get; }
        public Exception Exception { get; }
    }
}