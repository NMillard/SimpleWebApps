namespace CrudApp.Domain {
    public interface IEntity<out T> {
        public T Id { get; }
    }
}