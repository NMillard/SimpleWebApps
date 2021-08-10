using System;

namespace CrudApp.Domain {
    public record ChatMessage {
        public ChatMessage(string content) {
            Id = Guid.NewGuid();
            Content = content;
        }

        public Guid Id { get; }

        public string Content { get; }
    }
}