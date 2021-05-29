using System;

namespace CompositionOverInheritance.Composition {
    public class Schedule {
        private readonly ITriggerTime triggerTime;
        private readonly DateTimeOffset created;

        public Schedule(ITriggerTime triggerTime) {
            created = DateTimeOffset.UtcNow;
            this.triggerTime = triggerTime;
        }

        public bool IsDue() => triggerTime.IsDue(created);
    }
}