using System;

namespace BeginnerVsSenior {
    public class User {
        public User(string email) {
            Id = Guid.NewGuid();
            Email = email;
        }

        public Guid Id { get; }
        public string Email { get; set; }
    }
}