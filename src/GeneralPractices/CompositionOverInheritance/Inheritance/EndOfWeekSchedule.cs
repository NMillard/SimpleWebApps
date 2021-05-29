using System;

namespace CompositionOverInheritance.Inheritance {
    public sealed class EndOfWeekSchedule : ScheduleBase {
        public override bool IsDue() {
            // implementation
            throw new NotImplementedException();
        }
    }
}