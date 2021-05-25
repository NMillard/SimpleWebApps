using System;
using System.Collections.Generic;

namespace Polymorphism.WebApi.Users.Models {
    public class User {
        public User(string username) {
            Id = Guid.NewGuid();
            Username = username;
            Permissions = new List<Permission>();
        }

        public User(string username, IEnumerable<Permission> permissions) : this(username) {
            Permissions = permissions;
        }

        public Guid Id { get; }
        public string Username { get; private set; }
        public bool IsPremium { get; init; }
        public bool IsOnTrial { get; init; } // Not using this property yet
        public IEnumerable<Permission> Permissions { get; private set; }
    }
}