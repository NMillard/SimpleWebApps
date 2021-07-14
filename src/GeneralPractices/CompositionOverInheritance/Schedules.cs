using System;

namespace CompositionOverInheritance {
    /*
     * Simple class with basic switch statement.
     * If you're never going to change or add another schedule type, then this might be completely fine.
     */
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

        public DailySchedule() {
            created = DateTimeOffset.UtcNow;
        }

        public bool IsDue() {
            throw new NotImplementedException();
        }
    }

    public class EndOfWeekSchedule {
        private readonly DateTimeOffset created;

        public EndOfWeekSchedule() {
            created = DateTimeOffset.UtcNow;
        }

        public bool IsDue() {
            throw new NotImplementedException();
        }
    }
}