using System;
using DelegatingConfiguration;
using Microsoft.Extensions.Configuration;

/* 1. Rename daily schedule to Schedule
 * 2. Let it take ITriggerTime in constructor
 * 3. Implement triggers (Daily, EndOfWeek, => Weekly) - make Something2(Schedule)
 * 4. show how clients use it
 */


var config = new ConfigurationBuilder().Build();
config.BindSimpleSettings(out SomeSettings settings);


ScheduleBase dailySchedule = new DailySchedule(new TimeOnly(11, 0));
ScheduleBase eowSchedule = new EndOfWeekSchedule(new TimeOnly(11, 0));
ScheduleBase weeklySchedule = new WeeklySchedule(new TimeOnly(11, 0), DayOfWeek.Friday);

Schedule schedule = new Schedule(new TimeOnly(11, 0), new WeeklyTrigger(DayOfWeek.Friday));
Something2(schedule);

Something(weeklySchedule);

static void Something(ScheduleBase schedule) {
    schedule.IsDue();
}

static void Something2(Schedule schedule) {
    schedule.IsDue();
}

public class Schedule {
    private readonly TimeOnly time;
    private readonly ITriggerTime triggerTime;

    public Schedule(TimeOnly time, ITriggerTime triggerTime) {
        this.time = time;
        this.triggerTime = triggerTime;
    }

    public bool IsDue() => triggerTime.IsDue(time);
}

public interface ITriggerTime {
    bool IsDue(TimeOnly time);
}

public class DailyTrigger : ITriggerTime {
    public bool IsDue(TimeOnly time) => throw new NotImplementedException();
}

public class WeeklyTrigger : ITriggerTime {
    private readonly DayOfWeek dayOfWeek;

    public WeeklyTrigger(DayOfWeek dayOfWeek) => this.dayOfWeek = dayOfWeek;

    public bool IsDue(TimeOnly time) => throw new NotImplementedException();
}

public class EndOfWeekTrigger : WeeklyTrigger {
    public EndOfWeekTrigger() : base(DayOfWeek.Sunday) { }
 }


public abstract class ScheduleBase {
    protected readonly TimeOnly TriggerTime;
    
    protected ScheduleBase(TimeOnly triggerTime) {
        this.TriggerTime = triggerTime;
    }

    public abstract bool IsDue();
}

public class DailySchedule : ScheduleBase {
    public DailySchedule(TimeOnly triggerTime) : base(triggerTime) { }

    public override bool IsDue() => true;
}

public class WeeklySchedule : ScheduleBase {
    private readonly DayOfWeek dayOfWeek;

    public WeeklySchedule(TimeOnly triggerTime, DayOfWeek dayOfWeek) : base(triggerTime) {
        this.dayOfWeek = dayOfWeek;
    }

    public override bool IsDue() => true;
}

public class EndOfWeekSchedule : ScheduleBase {
    public EndOfWeekSchedule(TimeOnly triggerTime) : base(triggerTime) { }
    
    public override bool IsDue() => true;
}


#region factory
// void SwitchBased() {
//     var factory = new FactoryPattern.Factories.SwitchBased.ScheduleFactory();
//
//     // Schedule dailySchedule = factory.Create("Daily");
//     // Schedule endOfWeekSchedule = factory.Create("EndOfWeek");
// }
//
//
// void DictionaryBased() {
//     var factory = new FactoryPattern.Factories.DictionaryBased.ScheduleFactory();
//     factory.RegisterSchedule("daily", _ => new DailyTrigger());
//     factory.RegisterSchedule("endOfWeek", _ => new EndOfWeekTrigger());
//     factory.RegisterSchedule("weekly", parameters => new WeeklyTrigger(((WeeklyParameters) parameters).DayOfWeek));
//     // factory.RegisterSchedule("weekly-friday", _ => new Schedule(new WeeklyTrigger(DayOfWeek.Friday)));
//     
//     Schedule dailySchedule = factory.Create("Daily", ScheduleParameters.Empty);
//     Schedule endOfWeekSchedule = factory.Create("EndOfWeek", ScheduleParameters.Empty);
//     Schedule weeklySchedule = factory.Create("Weekly", new WeeklyParameters(DayOfWeek.Friday));
// }
//
// void OpenClosedBased() {
//     var factory = new FactoryPattern.Factories.FullyOpenClosed.ScheduleFactoryProvider();
//     factory.Create("daily", ScheduleParameters.Empty);
// }

#endregion

namespace Beginners {
    
    public class BeginnerSchedule {
        private readonly DateTimeOffset triggerTime;
        private readonly ScheduleType type;

        public BeginnerSchedule(DateTimeOffset triggerTime, ScheduleType type) {
            this.triggerTime = triggerTime;
            this.type = type;
        }

        public bool IsDue() => type switch {
            ScheduleType.Daily => throw new NotImplementedException(),
            ScheduleType.Weekly => throw new NotImplementedException(),
            ScheduleType.EndOfWeek => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public enum ScheduleType {
        Daily,
        Weekly,
        EndOfWeek,
    }
}