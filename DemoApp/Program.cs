using System;
/* 1. Rename daily schedule to Schedule
 * 2. Let it take ITriggerTime in constructor
 * 3. Implement triggers (Daily, EndOfWeek, => Weekly) - make Something2(Schedule)
 * 4. show how clients use it
 */

ScheduleBase dailySchedule = new DailySchedule();
ScheduleBase eowSchedule = new EndOfWeekSchedule();
ScheduleBase weeklySchedule = new WeeklySchedule(DayOfWeek.Friday);

Schedule schedule = new Schedule(new WeeklyTrigger(DayOfWeek.Friday));
Something2(schedule);

Something(weeklySchedule);

static void Something(ScheduleBase schedule) {
    schedule.IsDue();
}

static void Something2(Schedule schedule) {
    schedule.IsDue();
}

public class Schedule {
    private readonly ITriggerTime triggerTime;
    private readonly DateTimeOffset created;

    public Schedule(ITriggerTime triggerTime) {
        this.triggerTime = triggerTime;
        created = DateTimeOffset.UtcNow;
    }

    public bool IsDue() => triggerTime.IsDue(created);
}

public interface ITriggerTime {
    bool IsDue(DateTimeOffset activationDate);
}

public class DailyTrigger : ITriggerTime {
    public bool IsDue(DateTimeOffset activationDate) {
        throw new NotImplementedException();
    }
}

public class EndOfWeekTrigger : WeeklyTrigger {
    public EndOfWeekTrigger() : base(DayOfWeek.Sunday) { }
 }

public class WeeklyTrigger : ITriggerTime {
    private readonly DayOfWeek dayOfWeek;

    public WeeklyTrigger(DayOfWeek dayOfWeek) {
        this.dayOfWeek = dayOfWeek;
    }
    
    public bool IsDue(DateTimeOffset activactionDate) {
        throw new NotImplementedException();
    }
}





















public abstract class ScheduleBase {
    protected readonly DateTimeOffset created;

    public virtual bool IsDue() {
        // do important work
        throw new NotImplementedException();
    }
}

public class DailySchedule : ScheduleBase{
    public override bool IsDue() {
        throw new NotImplementedException();
    }
}

public class EndOfWeekSchedule : ScheduleBase {
    public override bool IsDue() { 
        throw new NotImplementedException();
    }
}

public class WeeklySchedule : ScheduleBase {
    private readonly DayOfWeek dayOfWeek;

    public WeeklySchedule(DayOfWeek dayOfWeek) {
        this.dayOfWeek = dayOfWeek;
    }

    public override bool IsDue() {
        throw new NotImplementedException();
    }
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
