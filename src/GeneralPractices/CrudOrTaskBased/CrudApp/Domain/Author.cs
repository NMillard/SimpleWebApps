using System;
using System.Collections.Generic;

namespace CrudApp.Domain {
    public class Author : IEntity<Guid> {
        public Author() {
            Id = Guid.NewGuid();
            PenNames = new List<string>();
        }
        
        public Guid Id { get; }
        public List<string> PenNames { get; set; }
        public string RealName { get; set; }
        public string Bio { get; set; }
        public string Email { get; set; }
    }
}