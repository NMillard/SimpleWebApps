using System;
using System.Collections.Generic;

namespace CrudApp.Domain {
    public class ChatChannel : IEntity<Guid> {
        private readonly List<ChatMessage> messages;

        public ChatChannel(Guid id, string name) {
            Id = id;
            Name = name;
            messages = new List<ChatMessage>();
        }

        public Guid Id { get; }
        public string Name { get; private set; }
        public IReadOnlyList<ChatMessage> ChatMessages => messages;

        public void PublishMessage(ChatMessage message) {
            messages.Add(message);
        }
    }
}