using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CompositionOverInheritance.Composition;
using FactoryPattern.Common;

namespace FactoryPattern.Common {
    public abstract record ScheduleParameters {
        public static EmptyParameters Empty => new();
    }

    public sealed record EmptyParameters : ScheduleParameters {
        internal EmptyParameters() {}
    }

    public sealed record EndOfWeekParameters(DayOfWeek DayOfWeek) : ScheduleParameters;
}

namespace FactoryPattern.SwitchBased {
    public class ScheduleFactory {
        public string[] RegisteredFactories => new[] { "Daily", "EndOfWeek" };
        public Schedule Create(string type, ScheduleParameters parameters) => type switch {
            "Daily" => new Schedule(new DailyTrigger()),
            "EndOfWeek" => new Schedule(new EndOfWeekTrigger()),
            "Weekly" when parameters is EndOfWeekParameters p => new Schedule(new WeeklyTrigger(p.DayOfWeek)),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}

namespace FactoryPattern.DictionaryBased {
    public class ScheduleFactory {
        private readonly Dictionary<string, Func<ScheduleParameters, Schedule>> factories;

        public ScheduleFactory() {
            factories = new Dictionary<string, Func<ScheduleParameters, Schedule>>();
        }
        
        public ScheduleFactory(Dictionary<string, Func<ScheduleParameters, Schedule>> factories) {
            this.factories = factories;
        }

        public string[] RegisteredFactories => factories.Keys.ToArray();
        
        public Schedule Create(string type, ScheduleParameters parameters) => factories[type](parameters);

        public void RegisterSchedule(string type, Func<ScheduleParameters, Schedule> factory) {
            if (string.IsNullOrEmpty(type)) return;
            factories[type] = factory;
        }
    }
}

namespace FactoryPattern.DynamicFactories {
    public class ScheduleFactoryProvider {
        private readonly Dictionary<string, Type> factories;

        public ScheduleFactoryProvider() {
            factories = Assembly.GetAssembly(typeof(ScheduleFactoryProvider))?
                .GetTypes()
                .Where(t => t.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(IScheduleFactory<>)))
                .ToDictionary(
                    keySelector: t => t.GetCustomAttribute<FactoryName>()?.Name.ToLower() ?? throw new ArgumentException()
                );
        }

        public string[] RegisteredFactories => factories.Keys.ToArray();

        public Schedule Create<T>(string factoryName, T parameters) where T : ScheduleParameters {
            Type factory = factories[factoryName.ToLower()];
            if (factory is null) throw new ArgumentException();
            
            var scheduleFactory = Activator.CreateInstance(factory) as IScheduleFactory<T>;
            return scheduleFactory?.Create(parameters) ?? throw new ArgumentException();
        }
    }
    

    public interface IScheduleFactory<in TParams> where TParams : ScheduleParameters {
        Schedule Create(TParams parameters);
    }

    [FactoryName("Daily")]
    public class DailyScheduleFactory : IScheduleFactory<EmptyParameters> {
        public Schedule Create(EmptyParameters parameters) => new(new DailyTrigger());
    }

    [FactoryName("EndOfWeek")]
    public class EndOfWeekScheduleFactory : IScheduleFactory<EmptyParameters> {
        public Schedule Create(EmptyParameters parameters) => new(new EndOfWeekTrigger());
    }

    [FactoryName("Weekly")]
    public class WeeklyScheduleFactory : IScheduleFactory<EndOfWeekParameters> {
        public Schedule Create(EndOfWeekParameters parameters) => new(new WeeklyTrigger(parameters.DayOfWeek));
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class FactoryName : Attribute {
        public FactoryName(string name) {
            Name = name;
        }
        
        public string Name { get; }
    }
}