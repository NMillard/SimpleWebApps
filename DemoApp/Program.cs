using System;
using CompositionOverInheritance.Composition;
using FactoryPattern.Common;
using FactoryPattern.DictionaryBased;
using FactoryPattern.DynamicFactories;

var provider = new ScheduleFactoryProvider();

Console.WriteLine(string.Join(" ", provider.RegisteredFactories));

var e = provider.Create("Weekly", new EndOfWeekParameters(DayOfWeek.Friday));

var fac = new ScheduleFactory();
fac.RegisterSchedule("daily", _ => new Schedule(new DailyTrigger()));
fac.RegisterSchedule("weekly", factory: parameters => {
    var p = parameters as EndOfWeekParameters;
    return new Schedule(new WeeklyTrigger(p.DayOfWeek));
});

var e3 = fac.Create("weekly", new EndOfWeekParameters(DayOfWeek.Friday));

Console.WriteLine();