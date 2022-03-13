using System;
using System.Collections.Generic;

namespace RepositoryPattern.Samples {
    public class User {
        private List<Book> books;

        public User(string username) {
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));
            
            Id = Guid.NewGuid();
            Username = username;
        }
        
        public Guid Id { get; }
        public string Username { get; }
        public IReadOnlyList<Book> Books => books;
    }
    
    public class Book {
        public Book(string title) {
            if (string.IsNullOrEmpty(title)) throw new ArgumentNullException(title);

            Id = Guid.NewGuid();
            Title = title;
        }
        
        public Guid Id { get; }
        public string Title { get; }
    }
}