using System;

namespace CompositionOverInheritance.Inheritance {

    public abstract class ScheduleBase {
        protected readonly DateTimeOffset created;

        protected ScheduleBase() => created = DateTimeOffset.UtcNow;

        public abstract bool IsDue();
    }
}