using System;

namespace CompositionOverInheritance.Inheritance {
    public sealed class DailySchedule : ScheduleBase {
        public override bool IsDue() {
            // implementation
            throw new NotImplementedException();
        }
    }
}