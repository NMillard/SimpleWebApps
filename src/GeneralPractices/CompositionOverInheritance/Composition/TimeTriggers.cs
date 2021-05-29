using System;

namespace CompositionOverInheritance.Composition {
    public interface ITriggerTime {
        bool IsDue(DateTimeOffset activationDate);
    }
    
    public sealed class DailyTrigger : ITriggerTime {
        public bool IsDue(DateTimeOffset activationDate) => throw new NotImplementedException();
    }
    
    public sealed class EndOfWeekTrigger : ITriggerTime {
        public bool IsDue(DateTimeOffset activationDate) => throw new NotImplementedException();
    }
    
    public sealed class WeeklyTrigger : ITriggerTime {
        private readonly DayOfWeek dayOfWeek;

        public WeeklyTrigger(DayOfWeek dayOfWeek) => this.dayOfWeek = dayOfWeek;

        public bool IsDue(DateTimeOffset activationDate) {
            throw new NotImplementedException();
        }
    }
}