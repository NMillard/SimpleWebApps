using System;
using CompositionOverInheritance.Composition;

namespace FactoryPattern.Factories.SwitchBased {
    public class ScheduleFactory {
        public string[] RegisteredFactories => new[] { "Daily", "EndOfWeek" };
        public Schedule Create(string type, ScheduleParameters parameters) {
            ITriggerTime trigger = type switch {
                "Daily" => new DailyTrigger(),
                "EndOfWeek" => new EndOfWeekTrigger(),
                "Weekly" when parameters is WeeklyParameters p => new WeeklyTrigger(p.DayOfWeek),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            return new Schedule(trigger);
        }
    }
}