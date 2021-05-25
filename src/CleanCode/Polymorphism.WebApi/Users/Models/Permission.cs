namespace Polymorphism.WebApi.Users.Models {
    public record Permission(string Type) {
        public static implicit operator Permission(string permission) => new(permission);
    }
}