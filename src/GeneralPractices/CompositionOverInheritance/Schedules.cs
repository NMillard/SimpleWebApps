using System;

namespace CompositionOverInheritance {
    public class Schedule {
        private readonly ScheduleType type;
        private readonly DateTimeOffset created;

        public Schedule(ScheduleType type) {
            created = DateTimeOffset.UtcNow;
            this.type = type;
        }

        public bool IsDue() {
            switch (type) {
                case ScheduleType.Daily:
                    // impl
                    break;
                case ScheduleType.EndOfWeek:
                    // impl
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            throw new NotImplementedException();
        }
    }

    public enum ScheduleType {
        Daily,
        EndOfWeek
    }


    public class DailySchedule {
        private readonly DateTimeOffset created;

        protected DailySchedule() => created = DateTimeOffset.UtcNow;

        public bool IsDue() => throw new NotImplementedException();
    }

    public class EndOfWeekSchedule {
        private readonly DateTimeOffset created;

        protected EndOfWeekSchedule() => created = DateTimeOffset.UtcNow;

        public bool IsDue() => throw new NotImplementedException();
    }
}