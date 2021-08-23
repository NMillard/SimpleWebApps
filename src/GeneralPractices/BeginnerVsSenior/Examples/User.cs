using System;

namespace BeginnerVsSenior {
    public class User {
        public User(string username) {
            Id = Guid.NewGuid();
            Username = username;
        }

        public Guid Id { get; }
        public Username Username { get; private set; }
    }

    public record Username {
        private readonly string value;

        public Username(string value) {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));
            this.value = value.Length switch {
                < 3 => throw new ArgumentException("Cannot be less than 3 characters"),
                > 50 => throw new ArgumentException("Cannot exceed 50 characters"),
                _ => value
            };
        }

        public static implicit operator Username(string value) => new(value);
        public static implicit operator string(Username username) => username.value;
    }
}